using Application.Interfaces.Common;
using Application.Interfaces.Projects;
using Application.Services.Base;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.WBS;
using Core.Enums.Projects;
using Core.Enums.Progress;
using Domain.Common;
using Domain.Entities.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Projects;

/// <summary>
/// Service implementation for Work Breakdown Structure operations
/// </summary>
public class WBSService : BaseService<WBSElement, WBSElementDto, CreateWBSElementDto, UpdateWBSElementDto>, IWBSService
{
    public WBSService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<WBSService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    #region Query Operations

    public async Task<PagedResult<WBSElementDto>> GetWBSElementsAsync(
        Guid projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.ControlAccount)
                .Include(w => w.WorkPackageDetails)
                .Where(w => w.ProjectId == projectId && !w.IsDeleted);

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                query = query.Where(w =>
                    w.Code.Contains(parameters.SearchTerm) ||
                    w.Name.Contains(parameters.SearchTerm) ||
                    (w.Description != null && w.Description.Contains(parameters.SearchTerm)));
            }

            // Apply sorting
            query = ApplySorting(query, parameters.SortBy, parameters.SortDirection);

            // Get total count
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<WBSElementDto>>(items);

            // Add additional computed properties
            foreach (var dto in dtos)
            {
                dto.CanHaveChildren = items.First(i => i.Id == dto.Id).CanHaveChildren();
                dto.ChildrenCount = await _unitOfWork.Repository<WBSElement>()
                    .Query()
                    .CountAsync(w => w.ParentId == dto.Id && !w.IsDeleted, cancellationToken);
            }

            return new PagedResult<WBSElementDto>(dtos, parameters.PageNumber, parameters.PageSize);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WBS elements for project: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<WBSElementDetailDto?> GetWBSElementByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.ControlAccount)
                .Include(w => w.WorkPackageDetails)
                    .ThenInclude(wp => wp!.ResponsibleUser)
                .Include(w => w.WorkPackageDetails)
                    .ThenInclude(wp => wp!.PrimaryDiscipline)
                .Include(w => w.CBSMappings)
                    .ThenInclude(m => m.CBS)
                .Include(w => w.Children.Where(c => !c.IsDeleted))
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, cancellationToken);

            if (element == null)
                return null;

            var dto = _mapper.Map<WBSElementDetailDto>(element);

            // Map dictionary info
            if (!string.IsNullOrWhiteSpace(element.DeliverableDescription) ||
                !string.IsNullOrWhiteSpace(element.AcceptanceCriteria))
            {
                dto.Dictionary = new WBSDictionaryDto
                {
                    WBSElementId = element.Id,
                    DeliverableDescription = element.DeliverableDescription,
                    AcceptanceCriteria = element.AcceptanceCriteria,
                    Assumptions = element.Assumptions,
                    Constraints = element.Constraints,
                    ExclusionsInclusions = element.ExclusionsInclusions
                };
            }

            dto.CanHaveChildren = element.CanHaveChildren();

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WBS element by id: {Id}", id);
            throw;
        }
    }

    public async Task<List<WBSElementDto>> GetWBSHierarchyAsync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var elements = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.ControlAccount)
                .Include(w => w.WorkPackageDetails)
                .Where(w => w.ProjectId == projectId && !w.IsDeleted)
                .OrderBy(w => w.Level)
                .ThenBy(w => w.SequenceNumber)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<WBSElementDto>>(elements);

            // Build hierarchy
            var hierarchy = BuildHierarchy(dtos);

            return hierarchy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WBS hierarchy for project: {ProjectId}", projectId);
            throw;
        }
    }

    public async Task<List<WBSElementDto>> GetChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var children = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.ControlAccount)
                .Include(w => w.WorkPackageDetails)
                .Where(w => w.ParentId == parentId && !w.IsDeleted)
                .OrderBy(w => w.SequenceNumber)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<WBSElementDto>>(children);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting children for WBS element: {ParentId}", parentId);
            throw;
        }
    }

    public async Task<List<WBSElementDto>> GetWorkPackagesByControlAccountAsync(
        Guid controlAccountId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var workPackages = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.WorkPackageDetails)
                .Where(w => w.ControlAccountId == controlAccountId &&
                           w.ElementType == WBSElementType.WorkPackage &&
                           !w.IsDeleted)
                .OrderBy(w => w.Code)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<WBSElementDto>>(workPackages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting work packages for control account: {ControlAccountId}", controlAccountId);
            throw;
        }
    }

    public async Task<WBSDictionaryDto?> GetWBSDictionaryAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .FirstOrDefaultAsync(w => w.Id == wbsElementId && !w.IsDeleted, cancellationToken);

            if (element == null)
                return null;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting WBS dictionary for element: {WBSElementId}", wbsElementId);
            throw;
        }
    }

    #endregion

    #region Command Operations

    public async Task<Result<Guid>> CreateWBSElementAsync(
        CreateWBSElementDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating WBS element with code: {Code} by user: {UserId}", dto.Code, userId);

            // Validate code uniqueness
            var validation = await ValidateWBSCodeAsync(dto.Code, dto.ProjectId, null, cancellationToken);
            if (!validation.IsValid)
                return Result<Guid>.Failure(validation.ErrorMessage!);

            // Calculate level and sequence
            int level = 0;
            int sequenceNumber = dto.SequenceNumber ?? 1;

            if (dto.ParentId.HasValue)
            {
                var parent = await _unitOfWork.Repository<WBSElement>()
                    .GetByIdAsync(dto.ParentId.Value);

                if (parent == null)
                    return Result<Guid>.Failure("Parent WBS element not found");

                if (!parent.CanHaveChildren())
                    return Result<Guid>.Failure("Parent element cannot have children");

                level = parent.Level + 1;

                // Auto-calculate sequence if not provided
                if (!dto.SequenceNumber.HasValue)
                {
                    sequenceNumber = await _unitOfWork.Repository<WBSElement>()
                        .Query()
                        .Where(w => w.ParentId == dto.ParentId && !w.IsDeleted)
                        .CountAsync(cancellationToken) + 1;
                }
            }

            var wbsElement = new WBSElement(
                dto.ProjectId,
                dto.Code,
                dto.Name,
                dto.ElementType,
                level,
                sequenceNumber,
                dto.ParentId);

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                wbsElement.UpdateBasicInfo(dto.Name, dto.Description);
            }

            await _unitOfWork.Repository<WBSElement>().AddAsync(wbsElement);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("WBS element created successfully: {Id}", wbsElement.Id);

            return Result<Guid>.Success(wbsElement.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating WBS element");
            return Result<Guid>.Failure($"Error creating WBS element: {ex.Message}");
        }
    }

    public async Task<r> UpdateWBSElementAsync(
        Guid id,
        UpdateWBSElementDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating WBS element: {Id} by user: {UserId}", id, userId);

            var element = await _unitOfWork.Repository<WBSElement>()
                .GetByIdAsync(id);

            if (element == null || element.IsDeleted)
                return Result.Failure("WBS element not found");

            element.UpdateBasicInfo(dto.Name, dto.Description);

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("WBS element updated successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating WBS element: {Id}", id);
            return Result.Failure($"Error updating WBS element: {ex.Message}");
        }
    }

    public async Task<r> UpdateWBSDictionaryAsync(
        Guid id,
        UpdateWBSDictionaryDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating WBS dictionary for element: {Id} by user: {UserId}", id, userId);

            var element = await _unitOfWork.Repository<WBSElement>()
                .GetByIdAsync(id);

            if (element == null || element.IsDeleted)
                return Result.Failure("WBS element not found");

            element.UpdateDictionaryInfo(
                dto.DeliverableDescription,
                dto.AcceptanceCriteria,
                dto.Assumptions,
                dto.Constraints,
                dto.ExclusionsInclusions);

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("WBS dictionary updated successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating WBS dictionary: {Id}", id);
            return Result.Failure($"Error updating WBS dictionary: {ex.Message}");
        }
    }

    public async Task<r> ConvertToWorkPackageAsync(
        Guid id,
        ConvertToWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Converting WBS element to work package: {Id} by user: {UserId}", id, userId);

            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, cancellationToken);

            if (element == null)
                return Result.Failure("WBS element not found");

            if (!await CanConvertToWorkPackageAsync(id, cancellationToken))
                return Result.Failure("Element cannot be converted to work package. It may have children or already be a work/planning package.");

            // Validate Control Account exists
            var controlAccount = await _unitOfWork.Repository<Domain.Entities.Cost.ControlAccount>()
                .GetByIdAsync(dto.ControlAccountId);

            if (controlAccount == null || controlAccount.IsDeleted)
                return Result.Failure("Control Account not found");

            // Convert to work package
            element.ConvertToWorkPackage(dto.ControlAccountId, dto.ProgressMethod);

            // Update work package details
            if (element.WorkPackageDetails != null)
            {
                element.WorkPackageDetails.UpdateSchedule(
                    dto.PlannedStartDate,
                    dto.PlannedEndDate,
                    null,
                    null);

                element.WorkPackageDetails.UpdateBudget(dto.Budget, dto.Currency);

                if (dto.ResponsibleUserId.HasValue)
                {
                    element.WorkPackageDetails.AssignResponsible(dto.ResponsibleUserId.Value);
                }

                if (dto.PrimaryDisciplineId.HasValue)
                {
                    element.WorkPackageDetails.AssignDiscipline(dto.PrimaryDisciplineId.Value);
                }

                if (dto.WeightFactor.HasValue)
                {
                    element.WorkPackageDetails.SetWeightFactor(dto.WeightFactor.Value);
                }
            }

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("WBS element converted to work package successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting to work package: {Id}", id);
            return Result.Failure($"Error converting to work package: {ex.Message}");
        }
    }

    public async Task<r> ConvertToPlanningPackageAsync(
        Guid id,
        Guid controlAccountId,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Converting WBS element to planning package: {Id} by user: {UserId}", id, userId);

            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, cancellationToken);

            if (element == null)
                return Result.Failure("WBS element not found");

            if (element.Children.Any())
                return Result.Failure("Cannot convert to planning package if element has children");

            // Validate Control Account exists
            var controlAccount = await _unitOfWork.Repository<Domain.Entities.Cost.ControlAccount>()
                .GetByIdAsync(controlAccountId);

            if (controlAccount == null || controlAccount.IsDeleted)
                return Result.Failure("Control Account not found");

            element.ConvertToPlanningPackage(controlAccountId);

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("WBS element converted to planning package successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting to planning package: {Id}", id);
            return Result.Failure($"Error converting to planning package: {ex.Message}");
        }
    }

    public async Task<r> ConvertPlanningPackageToWorkPackageAsync(
        Guid id,
        ConvertPlanningToWorkPackageDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Converting planning package to work package: {Id} by user: {UserId}", id, userId);

            var element = await _unitOfWork.Repository<WBSElement>()
                .GetByIdAsync(id);

            if (element == null || element.IsDeleted)
                return Result.Failure("Planning package not found");

            if (element.ElementType != WBSElementType.PlanningPackage)
                return Result.Failure("Element is not a planning package");

            element.ConvertPlanningPackageToWorkPackage(dto.ProgressMethod);

            // Update work package details
            if (element.WorkPackageDetails != null)
            {
                element.WorkPackageDetails.UpdateSchedule(
                    dto.PlannedStartDate,
                    dto.PlannedEndDate,
                    null,
                    null);

                element.WorkPackageDetails.UpdateBudget(dto.Budget, "USD");

                if (dto.ResponsibleUserId.HasValue)
                {
                    element.WorkPackageDetails.AssignResponsible(dto.ResponsibleUserId.Value);
                }

                if (dto.PrimaryDisciplineId.HasValue)
                {
                    element.WorkPackageDetails.AssignDiscipline(dto.PrimaryDisciplineId.Value);
                }

                if (dto.WeightFactor.HasValue)
                {
                    element.WorkPackageDetails.SetWeightFactor(dto.WeightFactor.Value);
                }
            }

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Planning package converted to work package successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error converting planning package to work package: {Id}", id);
            return Result.Failure($"Error converting planning package to work package: {ex.Message}");
        }
    }

    public async Task<r> ReorderWBSElementsAsync(
        ReorderWBSElementsDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Reordering WBS elements under parent: {ParentId} by user: {UserId}", dto.ParentId, userId);

            foreach (var order in dto.Elements)
            {
                var element = await _unitOfWork.Repository<WBSElement>()
                    .GetByIdAsync(order.Id);

                if (element != null && !element.IsDeleted && element.ParentId == dto.ParentId)
                {
                    element.UpdateSequenceNumber(order.SequenceNumber);
                    _unitOfWork.Repository<WBSElement>().Update(element);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("WBS elements reordered successfully");

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reordering WBS elements");
            return Result.Failure($"Error reordering WBS elements: {ex.Message}");
        }
    }

    public async Task<r> DeleteWBSElementAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting WBS element: {Id} by user: {UserId}", id, userId);

            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, cancellationToken);

            if (element == null)
                return Result.Failure("WBS element not found");

            if (!await CanDeleteWBSElementAsync(id, cancellationToken))
                return Result.Failure("Cannot delete WBS element. It may have children or active dependencies.");

            element.Delete(userId);

            _unitOfWork.Repository<WBSElement>().Update(element);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("WBS element deleted successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting WBS element: {Id}", id);
            return Result.Failure($"Error deleting WBS element: {ex.Message}");
        }
    }

    #endregion

    #region Validation Operations

    public async Task<bool> CanConvertToWorkPackageAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .FirstOrDefaultAsync(w => w.Id == wbsElementId && !w.IsDeleted, cancellationToken);

            if (element == null)
                return false;

            // Cannot convert if already a work package or planning package
            if (element.ElementType == WBSElementType.WorkPackage ||
                element.ElementType == WBSElementType.PlanningPackage)
                return false;

            // Cannot convert if has children
            if (element.Children.Any(c => !c.IsDeleted))
                return false;

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if can convert to work package: {Id}", wbsElementId);
            return false;
        }
    }

    public async Task<bool> CanDeleteWBSElementAsync(
        Guid wbsElementId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var element = await _unitOfWork.Repository<WBSElement>()
                .Query()
                .Include(w => w.Children)
                .FirstOrDefaultAsync(w => w.Id == wbsElementId && !w.IsDeleted, cancellationToken);

            if (element == null)
                return false;

            // Cannot delete if has active children
            if (element.Children.Any(c => !c.IsDeleted))
                return false;

            // Cannot delete if it's a work package with actuals
            if (element.WorkPackageDetails != null && element.WorkPackageDetails.ActualCost > 0)
                return false;

            // Add more validation rules as needed

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if can delete WBS element: {Id}", wbsElementId);
            return false;
        }
    }

    public async Task<ValidationResult> ValidateWBSCodeAsync(
        string code,
        Guid projectId,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Check format
            if (string.IsNullOrWhiteSpace(code))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "WBS code cannot be empty"
                };
            }

            // Check uniqueness
            var query = _unitOfWork.Repository<WBSElement>()
                .Query()
                .Where(w => w.ProjectId == projectId && w.Code == code && !w.IsDeleted);

            if (excludeId.HasValue)
            {
                query = query.Where(w => w.Id != excludeId.Value);
            }

            var exists = await query.AnyAsync(cancellationToken);

            if (exists)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "WBS code already exists in this project"
                };
            }

            // Validate code format (e.g., 1.2.3)
            var parts = code.Split('.');
            foreach (var part in parts)
            {
                if (!int.TryParse(part, out _))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "WBS code must be in format X.Y.Z (e.g., 1.2.3)"
                    };
                }
            }

            return new ValidationResult { IsValid = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating WBS code: {Code}", code);
            return new ValidationResult
            {
                IsValid = false,
                ErrorMessage = "Error validating WBS code"
            };
        }
    }

    #endregion

    #region Private Methods

    private IQueryable<WBSElement> ApplySorting(IQueryable<WBSElement> query, string? sortBy, string? sortDirection)
    {
        var isDescending = sortDirection?.ToLower() == "desc";

        return (sortBy?.ToLower()) switch
        {
            "code" => isDescending ? query.OrderByDescending(w => w.Code) : query.OrderBy(w => w.Code),
            "name" => isDescending ? query.OrderByDescending(w => w.Name) : query.OrderBy(w => w.Name),
            "level" => isDescending ? query.OrderByDescending(w => w.Level) : query.OrderBy(w => w.Level),
            "type" => isDescending ? query.OrderByDescending(w => w.ElementType) : query.OrderBy(w => w.ElementType),
            _ => query.OrderBy(w => w.Level).ThenBy(w => w.SequenceNumber)
        };
    }

    private List<WBSElementDto> BuildHierarchy(List<WBSElementDto> allElements)
    {
        var lookup = allElements.ToLookup(e => e.ParentId);
        var rootElements = lookup[null].ToList();

        foreach (var element in allElements)
        {
            element.ChildrenCount = lookup[element.Id].Count();
        }

        return rootElements;
    }

    #endregion
}