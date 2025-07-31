using Application.Interfaces.Auth;
using Application.Interfaces.Cost;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.ControlAccounts;
using Core.Enums.Cost;
using Core.Enums.Progress;
using Domain.Entities.Cost;
using Domain.Entities.EVM;
using Domain.Entities.Projects;
using Domain.Entities.Security;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using InvalidOperationException = Application.Common.Exceptions.InvalidOperationException;

namespace Application.Services.Cost;

/// <summary>
/// Implementation of Control Account service operations
/// </summary>
public class ControlAccountService : IControlAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUserContext _currentUser;
    private readonly ILogger<ControlAccountService> _logger;

    public ControlAccountService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ICurrentUserContext currentUser,
        ILogger<ControlAccountService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
        _logger = logger;
    }

    #region Query Operations

    public async Task<PagedResult<ControlAccountDto>> GetControlAccountsAsync(
        Guid? projectId,
        QueryParameters parameters,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Include(ca => ca.Project)
                .Include(ca => ca.Phase)
                .Include(ca => ca.CAMUser)
                .Where(ca => !ca.IsDeleted);

            // Apply project filter if provided
            if (projectId.HasValue)
            {
                query = query.Where(ca => ca.ProjectId == projectId.Value);
            }

            // Apply additional filters from parameters
            if (parameters.Filters.TryGetValue("status", out var status))
            {
                if (Enum.TryParse<ControlAccountStatus>(status, out var statusEnum))
                {
                    query = query.Where(ca => ca.Status == statusEnum);
                }
            }

            if (parameters.Filters.TryGetValue("phaseId", out var phaseId))
            {
                if (Guid.TryParse(phaseId, out var phaseGuid))
                {
                    query = query.Where(ca => ca.PhaseId == phaseGuid);
                }
            }

            if (parameters.Filters.TryGetValue("camUserId", out var camUserId))
            {
                query = query.Where(ca => ca.CAMUserId == camUserId);
            }

            if (parameters.Filters.TryGetValue("isActive", out var isActive))
            {
                if (bool.TryParse(isActive, out var isActiveBool))
                {
                    query = query.Where(ca => ca.IsActive == isActiveBool);
                }
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchLower = parameters.SearchTerm.ToLower();
                query = query.Where(ca =>
                    ca.Code.ToLower().Contains(searchLower) ||
                    ca.Name.ToLower().Contains(searchLower) ||
                    (ca.Description != null && ca.Description.ToLower().Contains(searchLower)));
            }

            // Apply sorting
            query = parameters.SortBy?.ToLower() switch
            {
                "code" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(ca => ca.Code)
                    : query.OrderBy(ca => ca.Code),
                "name" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(ca => ca.Name)
                    : query.OrderBy(ca => ca.Name),
                "bac" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(ca => ca.BAC)
                    : query.OrderBy(ca => ca.BAC),
                "percentcomplete" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(ca => ca.PercentComplete)
                    : query.OrderBy(ca => ca.PercentComplete),
                "status" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(ca => ca.Status)
                    : query.OrderBy(ca => ca.Status),
                "createdat" => parameters.SortDirection == "desc"
                    ? query.OrderByDescending(ca => ca.CreatedAt)
                    : query.OrderBy(ca => ca.CreatedAt),
                _ => query.OrderByDescending(ca => ca.CreatedAt)
            };

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination
            var items = await query
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            // Map to DTOs
            var dtos = _mapper.Map<List<ControlAccountDto>>(items);

            return new PagedResult<ControlAccountDto>
            {
                Items = dtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting control accounts");
            throw;
        }
    }

    public async Task<ControlAccountDetailDto?> GetControlAccountByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Include(ca => ca.Project)
                .Include(ca => ca.Phase)
                .Include(ca => ca.CAMUser)
                .Include(ca => ca.WorkPackages)
                    .ThenInclude(wp => wp.ResponsibleUser)
                .Include(ca => ca.PlanningPackages)
                .Include(ca => ca.Assignments)
                    .ThenInclude(a => a.UserId)
                .Include(ca => ca.EVMRecords.OrderByDescending(e => e.DataDate).Take(1))
                .FirstOrDefaultAsync(ca => ca.Id == id && !ca.IsDeleted, cancellationToken);

            if (controlAccount == null)
                return null;

            var dto = _mapper.Map<ControlAccountDetailDto>(controlAccount);

            // Calculate additional summary values
            dto.WorkPackageCount = controlAccount.WorkPackages.Count(wp => !wp.IsDeleted);
            dto.PlanningPackageCount = controlAccount.PlanningPackages.Count(pp => !pp.IsConverted);
            dto.ActualCost = controlAccount.WorkPackages
                .Where(wp => !wp.IsDeleted)
                .Sum(wp => wp.ActualCost);

            // Map latest EVM data if available
            var latestEVM = controlAccount.EVMRecords.FirstOrDefault();
            if (latestEVM != null)
            {
                dto.LatestEVM = _mapper.Map<EVMSummaryDto>(latestEVM);
                dto.EarnedValue = latestEVM.EV;
                dto.PlannedValue = latestEVM.PV;
                dto.CPI = latestEVM.CPI;
                dto.SPI = latestEVM.SPI;
            }
            else
            {
                // Set default values if no EVM data
                dto.EarnedValue = 0;
                dto.PlannedValue = 0;
                dto.CPI = 0;
                dto.SPI = 0;
            }

            return dto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting control account by id: {Id}", id);
            throw;
        }
    }

    public async Task<List<ControlAccountDto>> GetControlAccountsByPhaseAsync(
        Guid phaseId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var controlAccounts = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Include(ca => ca.Project)
                .Include(ca => ca.Phase)
                .Include(ca => ca.CAMUser)
                .Where(ca => ca.PhaseId == phaseId && !ca.IsDeleted && ca.IsActive)
                .OrderBy(ca => ca.Code)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ControlAccountDto>>(controlAccounts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting control accounts by phase: {PhaseId}", phaseId);
            throw;
        }
    }

    public async Task<List<ControlAccountAssignmentDto>> GetControlAccountAssignmentsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var assignments = await _unitOfWork.Repository<ControlAccountAssignment>()
                .Query()
                .Include(a => a.User)
                .Where(a => a.ControlAccountId == id && a.IsActive)
                .OrderBy(a => a.Role)
                .ThenBy(a => a.AssignedDate)
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<ControlAccountAssignmentDto>>(assignments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting control account assignments for: {Id}", id);
            throw;
        }
    }

    public async Task<EVMSummaryDto?> GetLatestEVMSummaryAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var evmRecord = await _unitOfWork.Repository<EVMRecord>()
                .Query()
                .Where(e => e.ControlAccountId == id)
                .OrderByDescending(e => e.DataDate)
                .FirstOrDefaultAsync(cancellationToken);

            return evmRecord != null ? _mapper.Map<EVMSummaryDto>(evmRecord) : null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest EVM summary for control account: {Id}", id);
            throw;
        }
    }

    #endregion

    #region Command Operations

    public async Task<Result<Guid>> CreateControlAccountAsync(
        CreateControlAccountDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Creating control account with code: {Code} by user: {UserId}", dto.Code, userId);

            // Validate code format (C-XXX-YY-CAM-##)
            if (!IsValidControlAccountCode(dto.Code))
                return Result<Guid>.Failure("Invalid Control Account code format. Expected format: C-XXX-YY-CAM-##");

            // Check if code already exists
            var existingCA = await _unitOfWork.Repository<ControlAccount>()
                .AnyAsync(ca => ca.Code == dto.Code, cancellationToken);

            if (existingCA)
                return Result<Guid>.Failure($"Control Account with code '{dto.Code}' already exists");

            // Validate project exists
            var project = await _unitOfWork.Repository<Project>()
                .GetByIdAsync(dto.ProjectId, cancellationToken);
            if (project == null)
                return Result<Guid>.Failure("Project not found");

            // Validate phase exists and belongs to project
            var phase = await _unitOfWork.Repository<Phase>()
                .FindSingleAsync(p => p.Id == dto.PhaseId && p.ProjectId == dto.ProjectId, cancellationToken);
            if (phase == null)
                return Result<Guid>.Failure("Phase not found or does not belong to the specified project");

            // Validate CAM user exists
            var camUser = await _unitOfWork.Repository<User>()
                .GetByIdAsync(Guid.Parse(dto.CAMUserId), cancellationToken);
            if (camUser == null)
                return Result<Guid>.Failure("Control Account Manager user not found");

            // Create control account
            var controlAccount = new ControlAccount(
                dto.Code,
                dto.Name,
                dto.ProjectId,
                dto.PhaseId,
                dto.CAMUserId,
                dto.BAC,
                dto.MeasurementMethod);

            controlAccount.UpdateBudget(dto.BAC, dto.ContingencyReserve, dto.ManagementReserve);

            if (!string.IsNullOrWhiteSpace(dto.Description))
                controlAccount.UpdateBasicInfo(dto.Name, dto.Description);

            controlAccount.CreatedBy = userId;

            await _unitOfWork.Repository<ControlAccount>().AddAsync(controlAccount, cancellationToken);

            // Create initial assignment for CAM
            var camAssignment = new ControlAccountAssignment(
                controlAccount.Id,
                dto.CAMUserId,
                ControlAccountRole.Manager,
                100); // CAM has 100% allocation by default

            camAssignment.CreatedBy = userId;

            await _unitOfWork.Repository<ControlAccountAssignment>().AddAsync(camAssignment, cancellationToken);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Control Account created successfully: {Code} with Id: {Id}", dto.Code, controlAccount.Id);

            return Result<Guid>.Success(controlAccount.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating control account");
            return Result<Guid>.Failure($"Error creating control account: {ex.Message}");
        }
    }

    public async Task<Result> UpdateControlAccountAsync(
        Guid id,
        UpdateControlAccountDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating control account: {Id} by user: {UserId}", id, userId);

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(id, cancellationToken);

            if (controlAccount == null || controlAccount.IsDeleted)
                return Result.Failure("Control Account not found");

            if (controlAccount.Status == ControlAccountStatus.Closed)
                return Result.Failure("Cannot update a closed Control Account");

            // Update basic info
            controlAccount.UpdateBasicInfo(dto.Name, dto.Description);

            // Update budget if provided
            if (dto.BAC.HasValue)
            {
                if (controlAccount.Status == ControlAccountStatus.Baselined)
                    return Result.Failure("Cannot update budget for a baselined Control Account. Create a change request instead.");

                controlAccount.UpdateBudget(
                    dto.BAC.Value,
                    dto.ContingencyReserve ?? controlAccount.ContingencyReserve,
                    dto.ManagementReserve ?? controlAccount.ManagementReserve);
            }

            // Update CAM if provided and different
            if (!string.IsNullOrWhiteSpace(dto.CAMUserId) && dto.CAMUserId != controlAccount.CAMUserId)
            {
                // Validate new CAM exists
                var newCAM = await _unitOfWork.Repository<User>()
                    .GetByIdAsync(Guid.Parse(dto.CAMUserId), cancellationToken);
                if (newCAM == null)
                    return Result.Failure("New Control Account Manager user not found");

                // Deactivate current CAM assignment
                var currentCAMAssignment = await _unitOfWork.Repository<ControlAccountAssignment>()
                    .FindSingleAsync(a => a.ControlAccountId == id &&
                                    a.Role == ControlAccountRole.Manager &&
                                    a.IsActive, cancellationToken);

                if (currentCAMAssignment != null)
                {
                    //currentCAMAssignment.Deactivate();// TODO
                    currentCAMAssignment.UpdatedBy = userId;
                    _unitOfWork.Repository<ControlAccountAssignment>().Update(currentCAMAssignment);
                }

                // Create new CAM assignment
                var newCAMAssignment = new ControlAccountAssignment(
                    id,
                    dto.CAMUserId,
                    ControlAccountRole.Manager,
                    100);

                newCAMAssignment.CreatedBy = userId;
                await _unitOfWork.Repository<ControlAccountAssignment>().AddAsync(newCAMAssignment, cancellationToken);

                // Update control account CAM
                controlAccount.UpdateCAM(dto.CAMUserId);
            }

            // Update measurement method if provided
            if (dto.MeasurementMethod.HasValue)
            {
                if (controlAccount.Status == ControlAccountStatus.InProgress)
                    return Result.Failure("Cannot change measurement method for an in-progress Control Account");

                controlAccount.UpdateMeasurementMethod(dto.MeasurementMethod.Value);
            }

            controlAccount.UpdatedBy = userId;
            controlAccount.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ControlAccount>().Update(controlAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Control Account updated successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating control account: {Id}", id);
            return Result.Failure($"Error updating control account: {ex.Message}");
        }
    }

    public async Task<Result> UpdateControlAccountStatusAsync(
        Guid id,
        UpdateControlAccountStatusDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating control account status: {Id} to {Status} by user: {UserId}",
                id, dto.NewStatus, userId);

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(id, cancellationToken);

            if (controlAccount == null || controlAccount.IsDeleted)
                return Result.Failure("Control Account not found");

            // Store old status for logging
            var oldStatus = controlAccount.Status;

            try
            {
                controlAccount.UpdateStatus(dto.NewStatus);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            controlAccount.UpdatedBy = userId;
            controlAccount.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ControlAccount>().Update(controlAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Control Account status updated successfully: {Id} from {OldStatus} to {NewStatus}",
                id, oldStatus, dto.NewStatus);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating control account status: {Id}", id);
            return Result.Failure($"Error updating control account status: {ex.Message}");
        }
    }

    public async Task<Result> AssignUserToControlAccountAsync(
        Guid id,
        CreateControlAccountAssignmentDto dto,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Assigning user {UserId} to control account: {Id} as {Role}",
                dto.UserId, id, dto.Role);

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(id, cancellationToken);

            if (controlAccount == null || controlAccount.IsDeleted)
                return Result.Failure("Control Account not found");

            // Validate user exists
            var user = await _unitOfWork.Repository<User>()
                .GetByIdAsync(Guid.Parse(dto.UserId), cancellationToken);
            if (user == null)
                return Result.Failure("User not found");

            // Check if user is already assigned with the same role
            var existingAssignment = await _unitOfWork.Repository<ControlAccountAssignment>()
                .AnyAsync(a => a.ControlAccountId == id &&
                             a.UserId == dto.UserId &&
                             a.Role == dto.Role &&
                             a.IsActive, cancellationToken);

            if (existingAssignment)
                return Result.Failure("User is already assigned to this Control Account with the same role");

            // Validate allocation percentage
            if (dto.AllocationPercentage.HasValue)
            {
                var currentAllocations = await _unitOfWork.Repository<ControlAccountAssignment>()
                    .Query()
                    .Where(a => a.UserId == dto.UserId && a.IsActive && a.AllocationPercentage.HasValue)
                    .SumAsync(a => a.AllocationPercentage!.Value, cancellationToken);

                if (currentAllocations + dto.AllocationPercentage.Value > 100)
                    return Result.Failure($"User allocation would exceed 100%. Current allocation: {currentAllocations}%");
            }

            // Handle CAM role special case
            if (dto.Role == ControlAccountRole.Manager)
            {
                var currentCAM = await _unitOfWork.Repository<ControlAccountAssignment>()
                    .FindSingleAsync(a => a.ControlAccountId == id &&
                                    a.Role == ControlAccountRole.Manager &&
                                    a.IsActive, cancellationToken);

                if (currentCAM != null)
                {
                    // currentCAM.Deactivate(); //TODO
                    currentCAM.UpdatedBy = userId;
                    _unitOfWork.Repository<ControlAccountAssignment>().Update(currentCAM);
                }

                controlAccount.UpdateCAM(dto.UserId);
                _unitOfWork.Repository<ControlAccount>().Update(controlAccount);
            }

            var assignment = new ControlAccountAssignment(
                id,
                dto.UserId,
                dto.Role,
                dto.AllocationPercentage);

            assignment.CreatedBy = userId;

            await _unitOfWork.Repository<ControlAccountAssignment>().AddAsync(assignment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User assigned successfully to control account: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning user to control account: {Id}", id);
            return Result.Failure($"Error assigning user to control account: {ex.Message}");
        }
    }

    public async Task<Result> UpdateControlAccountProgressAsync(
        Guid id,
        decimal percentComplete,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating control account progress: {Id} to {Percent}% by user: {UserId}",
                id, percentComplete, userId);

            if (percentComplete < 0 || percentComplete > 100)
                return Result.Failure("Percent complete must be between 0 and 100");

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(id, cancellationToken);

            if (controlAccount == null || controlAccount.IsDeleted)
                return Result.Failure("Control Account not found");

            if (controlAccount.Status != ControlAccountStatus.InProgress &&
                controlAccount.Status != ControlAccountStatus.Baselined)
                return Result.Failure("Control Account must be in progress or baselined to update progress");

            controlAccount.UpdateProgress(percentComplete);
            controlAccount.UpdatedBy = userId;
            controlAccount.UpdatedAt = DateTime.UtcNow;

            // If progress is 100%, consider updating status
            if (percentComplete == 100 && controlAccount.Status == ControlAccountStatus.InProgress)
            {
                controlAccount.UpdateStatus(ControlAccountStatus.UnderReview);
            }

            _unitOfWork.Repository<ControlAccount>().Update(controlAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Control Account progress updated successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating control account progress: {Id}", id);
            return Result.Failure($"Error updating control account progress: {ex.Message}");
        }
    }

    public async Task<Result> BaselineControlAccountAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Baselining control account: {Id} by user: {UserId}", id, userId);

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .GetByIdAsync(id, cancellationToken);

            if (controlAccount == null || controlAccount.IsDeleted)
                return Result.Failure("Control Account not found");

            try
            {
                controlAccount.Baseline();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            controlAccount.UpdatedBy = userId;

            _unitOfWork.Repository<ControlAccount>().Update(controlAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Control Account baselined successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error baselining control account: {Id}", id);
            return Result.Failure($"Error baselining control account: {ex.Message}");
        }
    }

    public async Task<Result> CloseControlAccountAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Closing control account: {Id} by user: {UserId}", id, userId);

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Include(ca => ca.WorkPackages)
                .Include(ca => ca.PlanningPackages)
                .FirstOrDefaultAsync(ca => ca.Id == id && !ca.IsDeleted, cancellationToken);

            if (controlAccount == null)
                return Result.Failure("Control Account not found");

            // Check if all work packages are completed
            var hasIncompleteWorkPackages = controlAccount.WorkPackages
                .Any(wp => !wp.IsDeleted &&
                          wp.Status != WorkPackageStatus.Completed &&
                          wp.Status != WorkPackageStatus.Cancelled);

            if (hasIncompleteWorkPackages)
                return Result.Failure("Cannot close Control Account with incomplete work packages");

            // Check if all planning packages are converted
            var hasUnconvertedPlanningPackages = controlAccount.PlanningPackages
                .Any(pp => !pp.IsConverted);

            if (hasUnconvertedPlanningPackages)
                return Result.Failure("Cannot close Control Account with unconverted planning packages");

            controlAccount.UpdateStatus(ControlAccountStatus.Closed);
            controlAccount.UpdatedBy = userId;
            controlAccount.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<ControlAccount>().Update(controlAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Control Account closed successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing control account: {Id}", id);
            return Result.Failure($"Error closing control account: {ex.Message}");
        }
    }

    public async Task<Result> DeleteControlAccountAsync(
        Guid id,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting control account: {Id} by user: {UserId}", id, userId);

            var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                .Query()
                .Include(ca => ca.WorkPackages)
                .Include(ca => ca.PlanningPackages)
                .FirstOrDefaultAsync(ca => ca.Id == id && !ca.IsDeleted, cancellationToken);

            if (controlAccount == null)
                return Result.Failure("Control Account not found");

            try
            {
                //controlAccount.Delete(userId);// TODO
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            _unitOfWork.Repository<ControlAccount>().Update(controlAccount);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Control Account deleted successfully: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting control account: {Id}", id);
            return Result.Failure($"Error deleting control account: {ex.Message}");
        }
    }

    public async Task<Result> RemoveUserFromControlAccountAsync(
        Guid id,
        string userToRemove,
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Removing user {UserToRemove} from control account: {Id} by user: {UserId}",
                userToRemove, id, userId);

            var assignment = await _unitOfWork.Repository<ControlAccountAssignment>()
                .FindSingleAsync(a => a.ControlAccountId == id &&
                                    a.UserId == userToRemove &&
                                    a.IsActive, cancellationToken);

            if (assignment == null)
                return Result.Failure("Assignment not found");

            // Cannot remove CAM without assigning a new one
            if (assignment.Role == ControlAccountRole.Manager)
                return Result.Failure("Cannot remove Control Account Manager. Please assign a new CAM first.");

            // assignment.Deactivate(); // TODO
            assignment.UpdatedBy = userId;

            _unitOfWork.Repository<ControlAccountAssignment>().Update(assignment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User removed successfully from control account: {Id}", id);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from control account: {Id}", id);
            return Result.Failure($"Error removing user from control account: {ex.Message}");
        }
    }

    #endregion

    #region Private Methods

    private bool IsValidControlAccountCode(string code)
    {
        // Validate format: C-XXX-YY-CAM-##
        if (string.IsNullOrWhiteSpace(code))
            return false;

        var parts = code.Split('-');
        if (parts.Length != 5)
            return false;

        // Check each part
        if (parts[0] != "C") return false;
        if (parts[1].Length != 3 || !parts[1].All(char.IsDigit)) return false;
        if (parts[2].Length != 2 || !parts[2].All(char.IsDigit)) return false;
        if (parts[3].Length != 3 || !parts[3].All(char.IsLetter)) return false;
        if (parts[4].Length != 2 || !parts[4].All(char.IsDigit)) return false;

        return true;
    }

    #endregion
}