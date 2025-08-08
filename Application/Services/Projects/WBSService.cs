using Application.Interfaces.Projects;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Projects.WBSElement;
using Core.DTOs.Projects.WorkPackageDetails;
using Core.Enums.Projects;
using Domain.Entities.WBS;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using ClosedXML.Excel;
using CsvHelper;

namespace Application.Services.Projects;

/// <summary>
/// Implementación del servicio para operaciones de Estructura de Desglose de Trabajo (WBS)
/// </summary>
public class WBSService : IWBSService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<WBSService> _logger;

    public WBSService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<WBSService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    // Operaciones de consulta
    public async Task<PagedResult<WBSElementDto>> GetWBSElementsAsync(
        Guid projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Obteniendo elementos WBS para proyecto {ProjectId}", projectId);

        var query = _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.ProjectId == projectId)
            .Include(w => w.Children)
            .Include(w => w.ControlAccount)
            .AsQueryable();

        // Apply filters if any
        if (!string.IsNullOrEmpty(parameters.SearchTerm))
        {
            query = query.Where(w => 
                w.Code.Contains(parameters.SearchTerm) ||
                w.Name.Contains(parameters.SearchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = parameters.SortBy?.ToLower() switch
        {
            "code" => !parameters.IsAscending 
                ? query.OrderByDescending(w => w.Code) 
                : query.OrderBy(w => w.Code),
            "name" => !parameters.IsAscending 
                ? query.OrderByDescending(w => w.Name) 
                : query.OrderBy(w => w.Name),
            _ => query.OrderBy(w => w.Code)
        };

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        var mappedItems = _mapper.Map<List<WBSElementDto>>(items);

        return new PagedResult<WBSElementDto>
        {
            Items = mappedItems,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)parameters.PageSize)
        };
    }

    public async Task<WBSElementDetailDto?> GetWBSElementByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting WBS element {Id}", id);

        var element = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.Id == id)
            .Include(w => w.Children)
            .Include(w => w.Parent)
            .Include(w => w.ControlAccount)
            .Include(w => w.WorkPackageDetails)
            .Include(w => w.CBSMappings)
                .ThenInclude(m => m.CBS)
            .FirstOrDefaultAsync(cancellationToken);

        return element != null ? _mapper.Map<WBSElementDetailDto>(element) : null;
    }

    public async Task<List<WBSElementDto>> GetWBSHierarchyAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting WBS hierarchy for project {ProjectId}", projectId);

        var elements = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.ProjectId == projectId)
            .Include(w => w.Children)
            .Include(w => w.ControlAccount)
            .ToListAsync(cancellationToken);

        // Build hierarchy starting from root elements
        var rootElements = elements.Where(e => e.ParentId == null).ToList();
        return _mapper.Map<List<WBSElementDto>>(rootElements);
    }

    public async Task<List<WBSElementDto>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting children for WBS element {ParentId}", parentId);

        var children = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.ParentId == parentId)
            .Include(w => w.ControlAccount)
            .OrderBy(w => w.Code)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<WBSElementDto>>(children);
    }

    public async Task<List<WBSElementDto>> GetWorkPackagesByControlAccountAsync(
        Guid controlAccountId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting work packages for control account {ControlAccountId}", controlAccountId);

        var workPackages = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.ControlAccountId == controlAccountId && 
                       w.ElementType == WBSElementType.WorkPackage)
            .Include(w => w.WorkPackageDetails)
            .OrderBy(w => w.Code)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<WBSElementDto>>(workPackages);
    }

    public async Task<WBSDictionaryDto?> GetWBSDictionaryAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting WBS dictionary for element {Id}", wbsElementId);

        var element = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.Id == wbsElementId)
            .FirstOrDefaultAsync(cancellationToken);

        if (element == null) return null;

        return new WBSDictionaryDto
        {
            WBSElementId = element.Id,
            DeliverableDescription = element.DeliverableDescription,
            AcceptanceCriteria = element.AcceptanceCriteria,
            Assumptions = element.Assumptions,
            Constraints = element.Constraints,
            ExclusionsInclusions = element.ExclusionsInclusions
        };
    }

    // Command operations
    public async Task<Result<Guid>> CreateWBSElementAsync(
        CreateWBSElementDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating WBS element for project {ProjectId}", dto.ProjectId);

            // Validate WBS code uniqueness
            var codeExists = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .AnyAsync(w => w.ProjectId == dto.ProjectId && w.Code == dto.Code, cancellationToken);

            if (codeExists)
            {
                return Result<Guid>.Failure($"El código WBS '{dto.Code}' ya existe en el proyecto");
            }

            // Calculate level and sequence number based on parent
            int level = 1;
            int sequenceNumber = 1;
            
            if (dto.ParentId.HasValue)
            {
                var parent = await _unitOfWork.Repository<WBSElement>()
                    .GetByIdAsync(dto.ParentId.Value, cancellationToken);
                
                if (parent != null)
                {
                    level = parent.Level + 1;
                    // Get next sequence number for this parent
                    var siblingCount = await _unitOfWork.Repository<WBSElement>()
                        .Query()
                        .CountAsync(w => w.ParentId == dto.ParentId, cancellationToken);
                    sequenceNumber = siblingCount + 1;
                }
            }
            else
            {
                // Root level - get next sequence number
                var rootCount = await _unitOfWork.Repository<WBSElement>()
                    .Query()
                    .CountAsync(w => w.ProjectId == dto.ProjectId && w.ParentId == null, cancellationToken);
                sequenceNumber = rootCount + 1;
            }

            // Create the WBS element using constructor
            var element = new WBSElement(
                dto.ProjectId,
                dto.Code,
                dto.Name,
                dto.ElementType,
                level,
                sequenceNumber,
                dto.ParentId);

            // Set additional properties
            if (!string.IsNullOrEmpty(dto.Description))
            {
                element.UpdateBasicInfo(dto.Name, dto.Description);
            }
            
            element.CreatedBy = userId;
            element.UpdatedBy = userId;

            await _unitOfWork.Repository<WBSElement>().AddAsync(element, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Created WBS element {Id} with code {Code}", element.Id, element.Code);
            return Result<Guid>.Success(element.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating WBS element");
            return Result<Guid>.Failure($"Error al crear el elemento WBS: {ex.Message}");
        }
    }

    public async Task<Result> UpdateWBSElementAsync(
        Guid id,
        UpdateWBSElementDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating WBS element {Id}", id);

            var element = await _unitOfWork.Repository<WBSElement>()
                .GetByIdAsync(id, cancellationToken);

            if (element == null)
            {
                return Result.Failure($"Elemento WBS {id} no encontrado");
            }

            // Update properties using domain method
            element.UpdateBasicInfo(dto.Name, dto.Description);
            element.UpdatedBy = userId;
            // Note: ControlAccountId cannot be updated directly - it's managed through conversion methods

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated WBS element {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating WBS element {Id}", id);
            return Result.Failure($"Error al actualizar el elemento WBS: {ex.Message}");
        }
    }

    public async Task<Result> UpdateWBSDictionaryAsync(
        Guid id,
        UpdateWBSDictionaryDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating WBS dictionary for element {Id}", id);

            var element = await _unitOfWork.Repository<WBSElement>()
                .GetByIdAsync(id, cancellationToken);

            if (element == null)
            {
                return Result.Failure($"Elemento WBS {id} no encontrado");
            }

            // Update dictionary fields using domain method
            element.UpdateDictionaryInfo(
                dto.DeliverableDescription,
                dto.AcceptanceCriteria,
                dto.Assumptions,
                dto.Constraints,
                dto.ExclusionsInclusions);
            element.UpdatedBy = userId;

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Updated WBS dictionary for element {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating WBS dictionary for element {Id}", id);
            return Result.Failure($"Error al actualizar el diccionario WBS: {ex.Message}");
        }
    }

    public async Task<Result> ConvertToWorkPackageAsync(
        Guid id,
        ConvertToWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Converting WBS element {Id} to work package", id);

            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (element == null)
            {
                return Result.Failure($"Elemento WBS {id} no encontrado");
            }

            // Validar si se puede convertir
            if (element.Children.Any())
            {
                return Result.Failure("No se puede convertir a paquete de trabajo: el elemento tiene hijos");
            }

            if (element.ElementType == WBSElementType.WorkPackage)
            {
                return Result.Failure("El elemento ya es un paquete de trabajo");
            }

            // Convert to work package
            element.ConvertToWorkPackage(dto.ControlAccountId, dto.ProgressMethod);
            element.UpdatedBy = userId;
            
            if (dto.ResponsibleUserId.HasValue && element.WorkPackageDetails != null)
            {
                element.WorkPackageDetails.AssignResponsibility(dto.ResponsibleUserId.Value, null, userId);
            }
            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Converted WBS element {Id} to work package", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting WBS element {Id} to work package", id);
            return Result.Failure($"Error al convertir a paquete de trabajo: {ex.Message}");
        }
    }

    public async Task<Result> ConvertToPlanningPackageAsync(
        Guid id,
        Guid controlAccountId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Converting WBS element {Id} to planning package", id);

            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (element == null)
            {
                return Result.Failure($"Elemento WBS {id} no encontrado");
            }

            // Validate can convert
            if (element.Children.Any())
            {
                return Result.Failure("No se puede convertir a paquete de planificación: el elemento tiene hijos");
            }

            if (element.ElementType == WBSElementType.PlanningPackage)
            {
                return Result.Failure("El elemento ya es un paquete de planificación");
            }

            // Convert to planning package
            element.ConvertToPlanningPackage(controlAccountId);
            element.UpdatedBy = userId;

            // Note: Planning package entry creation removed - requires PhaseId which is not available in this context
            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Converted WBS element {Id} to planning package", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting WBS element {Id} to planning package", id);
            return Result.Failure($"Error al convertir a paquete de planificación: {ex.Message}");
        }
    }

    public async Task<Result> ConvertPlanningPackageToWorkPackageAsync(
        Guid id,
        ConvertPlanningToWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Converting planning package WBS element {Id} to work package", id);

            // Get WBS element that is a planning package
            var wbsElement = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .FirstOrDefaultAsync(w => w.Id == id && w.ElementType == WBSElementType.PlanningPackage, cancellationToken);

            if (wbsElement == null)
            {
                return Result.Failure($"Elemento WBS de paquete de planificación {id} no encontrado");
            }

            // Convert planning package to work package using domain method
            wbsElement.ConvertPlanningPackageToWorkPackage(dto.ProgressMethod);
            wbsElement.UpdatedBy = userId;

            // Assign responsibility if provided
            if (dto.ResponsibleUserId.HasValue && wbsElement.WorkPackageDetails != null)
            {
                wbsElement.WorkPackageDetails.AssignResponsibility(dto.ResponsibleUserId.Value, dto.PrimaryDisciplineId, userId);
            }

            _unitOfWork.Repository<WBSElement>().Update(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Converted planning package {Id} to work package", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting planning package {Id} to work package", id);
            return Result.Failure($"Error al convertir paquete de planificación: {ex.Message}");
        }
    }

    public async Task<Result> ReorderWBSElementsAsync(
        ReorderWBSElementsDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Reordering WBS elements");

            foreach (var item in dto.Elements)
            {
                var element = await _unitOfWork.Repository<WBSElement>()
                    .GetByIdAsync(item.Id, cancellationToken);

                if (element != null)
                {
                    element.UpdateSequenceNumber(item.SequenceNumber);
                    element.UpdatedBy = userId;
                    _unitOfWork.Repository<WBSElement>().Update(element);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Reordered {Count} WBS elements", dto.Elements.Count);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering WBS elements");
            return Result.Failure($"Error al reordenar elementos WBS: {ex.Message}");
        }
    }

    public async Task<Result> DeleteWBSElementAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting WBS element {Id}", id);

            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .Include(w => w.WorkPackageDetails)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (element == null)
            {
                return Result.Failure($"Elemento WBS {id} no encontrado");
            }

            // Validate can delete
            if (element.Children.Any())
            {
                return Result.Failure("No se puede eliminar: el elemento tiene hijos");
            }

            if (element.WorkPackageDetails != null)
            {
                return Result.Failure("No se puede eliminar: el elemento es un paquete de trabajo con datos");
            }

            _unitOfWork.Repository<WBSElement>().Remove(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted WBS element {Id}", id);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting WBS element {Id}", id);
            return Result.Failure($"Error al eliminar elemento WBS: {ex.Message}");
        }
    }

    // Validation operations
    public async Task<bool> CanConvertToWorkPackageAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default)
    {
        var element = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Include(w => w.Children)
            .FirstOrDefaultAsync(w => w.Id == wbsElementId, cancellationToken);

        if (element == null) return false;

        return !element.Children.Any() && 
               element.ElementType != WBSElementType.WorkPackage &&
               element.ElementType != WBSElementType.PlanningPackage;
    }

    public async Task<bool> CanDeleteWBSElementAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default)
    {
        var element = await _unitOfWork.Repository<WBSElement>()
            .Query()
            .Include(w => w.Children)
            .Include(w => w.WorkPackageDetails)
            .FirstOrDefaultAsync(w => w.Id == wbsElementId, cancellationToken);

        if (element == null) return false;

        return !element.Children.Any() && element.WorkPackageDetails == null;
    }

    public async Task<Application.Interfaces.Common.ValidationResult> ValidateWBSCodeAsync(
        string code,
        Guid projectId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<WBSElement>()
            .Query()
            .Where(w => w.ProjectId == projectId && w.Code == code);

        if (excludeId.HasValue)
        {
            query = query.Where(w => w.Id != excludeId.Value);
        }

        var exists = await query.AnyAsync(cancellationToken);

        return exists 
            ? new ValidationResult { IsValid = false, ErrorMessage = $"El código WBS '{code}' ya existe en el proyecto" }
            : new ValidationResult { IsValid = true };
    }

    // Bulk operations
    public async Task<BulkOperationResult> BulkCreateWBSElementsAsync(
        BulkCreateWBSElementsDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Bulk creating {Count} WBS elements", dto.Elements.Count);

        var result = new BulkOperationResult();
        var createdIds = new List<Guid>();
        var errors = new List<BulkOperationError>();

        // Group elements by parent to ensure correct hierarchy creation
        var elementsByParent = dto.Elements
            .GroupBy(e => e.ParentId)
            .OrderBy(g => g.Key == null ? 0 : 1); // Process root elements first

        foreach (var group in elementsByParent)
        {
            foreach (var elementDto in group)
            {
                try
                {
                    // Validate if parent exists (if specified)
                    if (elementDto.ParentId.HasValue && !createdIds.Contains(elementDto.ParentId.Value))
                    {
                        var parentExists = await _unitOfWork.Repository<WBSElement>()
                            .Query()
                            .AnyAsync(w => w.Id == elementDto.ParentId.Value, cancellationToken);

                        if (!parentExists)
                        {
                            errors.Add(new BulkOperationError
                            {
                                Id = Guid.Empty, // No tenemos un ID aún ya que no se creó
                                ErrorMessage = $"Elemento {elementDto.Code}: Elemento padre {elementDto.ParentId} no encontrado"
                            });
                            continue;
                        }
                    }

                    var createResult = await CreateWBSElementAsync(elementDto, userId, cancellationToken);
                    
                    if (createResult.IsSuccess)
                    {
                        result.SuccessCount++;
                        createdIds.Add(createResult.Value);
                    }
                    else
                    {
                        result.FailureCount++;
                        errors.Add(new BulkOperationError
                        {
                            Id = Guid.Empty,
                            ErrorMessage = $"Elemento {elementDto.Code}: {createResult.Error ?? "Error desconocido"}"
                        });
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating WBS element {Code}", elementDto.Code);
                    result.FailureCount++;
                    errors.Add(new BulkOperationError
                    {
                        Id = Guid.Empty,
                        ErrorMessage = $"Elemento {elementDto.Code}: {ex.Message}"
                    });
                }
            }
        }

        result.Errors = errors;
        _logger.LogInformation("Bulk creation completed: {Success} succeeded, {Failed} failed", 
            result.SuccessCount, result.FailureCount);

        return result;
    }

    // Import/Export operations
    public async Task<ImportResult> ImportWBSAsync(
        Stream fileStream,
        string fileName,
        Guid projectId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Importing WBS from file {FileName} for project {ProjectId}", fileName, projectId);

        try
        {
            // Determine file type
            var extension = Path.GetExtension(fileName)?.ToLower();
            
            if (extension != ".xlsx" && extension != ".csv")
            {
                return new ImportResult(0, 0, new List<string> { "Solo se admiten archivos Excel (.xlsx) o CSV (.csv)" });
            }

            var elementsToCreate = new List<CreateWBSElementDto>();
            var errors = new List<string>();

            if (extension == ".csv")
            {
                elementsToCreate = await ParseCsvFileAsync(fileStream, projectId, errors);
            }
            else
            {
                elementsToCreate = await ParseExcelFileAsync(fileStream, projectId, errors);
            }

            if (errors.Any())
            {
                return new ImportResult(0, errors.Count, errors);
            }

            // Create elements using bulk operation
            var bulkDto = new BulkCreateWBSElementsDto { Elements = elementsToCreate };
            var bulkResult = await BulkCreateWBSElementsAsync(bulkDto, userId, cancellationToken);

            return new ImportResult(
                bulkResult.SuccessCount,
                bulkResult.FailureCount,
                bulkResult.Errors.Select(e => $"{e.Id}: {e.ErrorMessage}").ToList());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error importing WBS file");
            return new ImportResult(0, 0, new List<string> { $"Error al importar archivo: {ex.Message}" });
        }
    }

    public async Task<byte[]> ExportWBSAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Exporting WBS for project {ProjectId}", projectId);

        try
        {
            // Get all WBS elements for the project
            var elements = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Where(w => w.ProjectId == projectId)
                .Include(w => w.Parent)
                .Include(w => w.ControlAccount)
                .Include(w => w.WorkPackageDetails)
                .OrderBy(w => w.FullPath)
                .ToListAsync(cancellationToken);

            if (!elements.Any())
            {
                throw new InvalidOperationException("No hay elementos WBS para exportar");
            }

            // Create Excel file using ClosedXML or similar library
            using var workbook = new ClosedXML.Excel.XLWorkbook();
            var worksheet = workbook.Worksheets.Add("WBS");

            // Add headers
            var headers = new[]
            {
                "Código", "Nombre", "Descripción", "Tipo", "Nivel", "Código Padre",
                "Cuenta de Control", "Estado", "Presupuesto", "Costo Real", 
                "Progreso %", "Fecha Inicio Plan", "Fecha Fin Plan"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = headers[i];
                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
            }

            // Add data
            int row = 2;
            foreach (var element in elements)
            {
                worksheet.Cell(row, 1).Value = element.Code;
                worksheet.Cell(row, 2).Value = element.Name;
                worksheet.Cell(row, 3).Value = element.Description ?? "";
                worksheet.Cell(row, 4).Value = element.ElementType.ToString();
                worksheet.Cell(row, 5).Value = element.Level;
                worksheet.Cell(row, 6).Value = element.Parent?.Code ?? "";
                worksheet.Cell(row, 7).Value = element.ControlAccount?.Code ?? "";
                worksheet.Cell(row, 8).Value = element.IsActive ? "Activo" : "Inactivo";
                
                if (element.WorkPackageDetails != null)
                {
                    worksheet.Cell(row, 9).Value = element.WorkPackageDetails.Budget;
                    worksheet.Cell(row, 10).Value = element.WorkPackageDetails.ActualCost;
                    worksheet.Cell(row, 11).Value = element.WorkPackageDetails.ProgressPercentage;
                    worksheet.Cell(row, 12).Value = element.WorkPackageDetails.PlannedStartDate.ToString("yyyy-MM-dd");
                    worksheet.Cell(row, 13).Value = element.WorkPackageDetails.PlannedEndDate.ToString("yyyy-MM-dd");
                }

                row++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            // Convert to byte array
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error exporting WBS for project {ProjectId}", projectId);
            throw;
        }
    }

    // Helper methods for import
    private async Task<List<CreateWBSElementDto>> ParseCsvFileAsync(
        Stream fileStream, 
        Guid projectId, 
        List<string> errors)
    {
        var elements = new List<CreateWBSElementDto>();
        
        using var reader = new StreamReader(fileStream);
        using var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture);
        
        // Skip header
        await csv.ReadAsync();
        await csv.ReadAsync();
        
        int row = 2;
        while (await csv.ReadAsync())
        {
            try
            {
                var element = new CreateWBSElementDto
                {
                    ProjectId = projectId,
                    Code = csv.GetField<string>(0) ?? throw new Exception("El código es requerido"),
                    Name = csv.GetField<string>(1) ?? throw new Exception("El nombre es requerido"),
                    Description = csv.GetField<string>(2),
                    ElementType = Enum.Parse<WBSElementType>(csv.GetField<string>(3) ?? "Level"),
                    ParentId = null // Will be resolved later based on parent code
                };
                
                elements.Add(element);
            }
            catch (Exception ex)
            {
                errors.Add($"Fila {row}: {ex.Message}");
            }
            row++;
        }
        
        return elements;
    }

    private async Task<List<CreateWBSElementDto>> ParseExcelFileAsync(
        Stream fileStream, 
        Guid projectId, 
        List<string> errors)
    {
        var elements = new List<CreateWBSElementDto>();
        
        using var workbook = new ClosedXML.Excel.XLWorkbook(fileStream);
        var worksheet = workbook.Worksheet(1);
        
        var rows = worksheet.RowsUsed().Skip(1); // Skip header
        int rowNumber = 2;
        
        foreach (var row in rows)
        {
            try
            {
                var element = new CreateWBSElementDto
                {
                    ProjectId = projectId,
                    Code = row.Cell(1).GetValue<string>() ?? throw new Exception("El código es requerido"),
                    Name = row.Cell(2).GetValue<string>() ?? throw new Exception("El nombre es requerido"),
                    Description = row.Cell(3).GetValue<string>(),
                    ElementType = Enum.Parse<WBSElementType>(row.Cell(4).GetValue<string>() ?? "Level"),
                    ParentId = null // Will be resolved later based on parent code
                };
                
                elements.Add(element);
            }
            catch (Exception ex)
            {
                errors.Add($"Fila {rowNumber}: {ex.Message}");
            }
            rowNumber++;
        }
        
        return await Task.FromResult(elements);
    }
}