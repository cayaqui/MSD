using Application.Interfaces.Cost;
using Application.Interfaces.Common;
using Application.Interfaces.Auth;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Cost.ControlAccounts;
using Domain.Common;
using Domain.Entities.Cost.Control;
using Domain.Entities.WBS;
using Domain.Entities.Cost.EVM;
using Domain.Entities.Auth.Security;
using Microsoft.EntityFrameworkCore;
using Core.Enums.Cost;
using Core.Enums.Progress;

namespace Application.Services.Cost
{
    /// <summary>
    /// Service implementation for Control Account operations
    /// </summary>
    public class ControlAccountService : IControlAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public ControlAccountService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<ControlAccountDto>> GetControlAccountsAsync(
            Guid? projectId,
            QueryParameters parameters,
            CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.Repository<ControlAccount>().Query();

            if (projectId.HasValue)
            {
                query = query.Where(ca => ca.ProjectId == projectId.Value);
            }

            query = query.Where(ca => !ca.IsDeleted && ca.IsActive);

            // Apply search
            if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
            {
                var searchTerm = parameters.SearchTerm.ToLower();
                query = query.Where(ca =>
                    ca.Code.ToLower().Contains(searchTerm) ||
                    ca.Name.ToLower().Contains(searchTerm) ||
                    (ca.Description != null && ca.Description.ToLower().Contains(searchTerm)));
            }

            // Apply sorting
            var isDescending = !parameters.IsAscending;
            query = parameters.SortBy?.ToLower() switch
            {
                "code" => isDescending ? query.OrderByDescending(ca => ca.Code) : query.OrderBy(ca => ca.Code),
                "name" => isDescending ? query.OrderByDescending(ca => ca.Name) : query.OrderBy(ca => ca.Name),
                "status" => isDescending ? query.OrderByDescending(ca => ca.Status) : query.OrderBy(ca => ca.Status),
                "percentcomplete" => isDescending ? query.OrderByDescending(ca => ca.PercentComplete) : query.OrderBy(ca => ca.PercentComplete),
                _ => isDescending ? query.OrderByDescending(ca => ca.CreatedAt) : query.OrderBy(ca => ca.CreatedAt)
            };

            var totalItems = await query.CountAsync(cancellationToken);

            var items = await query
                .Include(ca => ca.Project)
                .Include(ca => ca.Phase)
                .Include(ca => ca.CAMUser)
                .Skip(parameters.Skip)
                .Take(parameters.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = new List<ControlAccountDto>();
            foreach (var item in items)
            {
                var dto = _mapper.Map<ControlAccountDto>(item);
                MapComputedProperties(dto, item);
                dtos.Add(dto);
            }

            return new PagedResult<ControlAccountDto>
            {
                Items = dtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)parameters.PageSize)
            };
        }

        public async Task<ControlAccountDetailDto?> GetControlAccountByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Repository<ControlAccount>()
                .GetAsync(
                    filter: ca => ca.Id == id && !ca.IsDeleted,
                    includeProperties: "Project,Phase,CAMUser,WorkPackages.WorkPackageDetails,PlanningPackages,Assignments.User,EVMRecords",
                    cancellationToken: cancellationToken);

            if (entity == null)
                return null;

            var dto = _mapper.Map<ControlAccountDetailDto>(entity);
            MapDetailProperties(dto, entity);

            return dto;
        }

        public async Task<List<ControlAccountDto>> GetControlAccountsByPhaseAsync(
            Guid phaseId,
            CancellationToken cancellationToken = default)
        {
            var entities = await _unitOfWork.Repository<ControlAccount>()
                .GetAllAsync(
                    filter: ca => ca.PhaseId == phaseId && !ca.IsDeleted && ca.IsActive,
                    includeProperties: "Project,Phase,CAMUser",
                    cancellationToken: cancellationToken);

            var dtos = new List<ControlAccountDto>();
            foreach (var entity in entities)
            {
                var dto = _mapper.Map<ControlAccountDto>(entity);
                MapComputedProperties(dto, entity);
                dtos.Add(dto);
            }

            return dtos;
        }

        public async Task<List<ControlAccountAssignmentDto>> GetControlAccountAssignmentsAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var assignments = await _unitOfWork.Repository<ControlAccountAssignment>()
                .GetAllAsync(
                    filter: a => a.ControlAccountId == id && a.IsActive,
                    includeProperties: "User",
                    cancellationToken: cancellationToken);

            return _mapper.Map<List<ControlAccountAssignmentDto>>(assignments);
        }

        public async Task<EVMSummaryDto?> GetLatestEVMSummaryAsync(
            Guid id,
            CancellationToken cancellationToken = default)
        {
            var evmRecords = await _unitOfWork.Repository<EVMRecord>()
                .GetAllAsync(
                    filter: e => e.ControlAccountId == id,
                    orderBy: q => q.OrderByDescending(e => e.DataDate),
                    cancellationToken: cancellationToken);

            var evmRecord = evmRecords.FirstOrDefault();
            if (evmRecord == null)
                return null;

            return _mapper.Map<EVMSummaryDto>(evmRecord);
        }

        public async Task<Result<Guid>> CreateControlAccountAsync(
            CreateControlAccountDto dto,
            string userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Validate code uniqueness
                var existingCode = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Code == dto.Code && !ca.IsDeleted,
                        cancellationToken: cancellationToken);

                if (existingCode != null)
                    return Result<Guid>.Failure("Control Account code already exists");

                // Validate CAM user exists
                var camUser = await _unitOfWork.Repository<User>()
                    .GetAsync(
                        filter: u => u.Id.ToString() == dto.CAMUserId && !u.IsDeleted,
                        cancellationToken: cancellationToken);

                if (camUser == null)
                    return Result<Guid>.Failure("Control Account Manager user not found");

                // Create Control Account
                var entity = new ControlAccount(
                    dto.Code,
                    dto.Name,
                    dto.ProjectId,
                    dto.PhaseId,
                    camUser.Id,
                    dto.BAC,
                    dto.MeasurementMethod);

                if (!string.IsNullOrWhiteSpace(dto.Description))
                {
                    entity.UpdateBasicInfo(dto.Name, dto.Description);
                }

                entity.UpdateBudget(dto.BAC, dto.ContingencyReserve, dto.ManagementReserve);

                entity.CreatedBy = userId;
                entity.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<ControlAccount>().AddAsync(entity, cancellationToken);

                // Create CAM assignment
                var assignment = new ControlAccountAssignment(
                    entity.Id,
                    camUser.Id,
                    ControlAccountRole.Manager,
                    100);

                assignment.CreatedBy = userId;
                assignment.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<ControlAccountAssignment>().AddAsync(assignment, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<Guid>.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return Result<Guid>.Failure($"Failed to create Control Account: {ex.Message}");
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
                var entity = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == id && !ca.IsDeleted,
                        cancellationToken: cancellationToken);

                if (entity == null)
                    return Result.Failure("Control Account not found");

                // Update basic info
                entity.UpdateBasicInfo(dto.Name, dto.Description);

                // Update budget if provided
                if (dto.BAC.HasValue || dto.ContingencyReserve.HasValue || dto.ManagementReserve.HasValue)
                {
                    entity.UpdateBudget(
                        dto.BAC ?? entity.BAC,
                        dto.ContingencyReserve ?? entity.ContingencyReserve,
                        dto.ManagementReserve ?? entity.ManagementReserve);
                }

                // Update CAM if provided
                if (!string.IsNullOrWhiteSpace(dto.CAMUserId))
                {
                    var camUser = await _unitOfWork.Repository<User>()
                        .GetAsync(
                            filter: u => u.Id.ToString() == dto.CAMUserId && !u.IsDeleted,
                            cancellationToken: cancellationToken);

                    if (camUser == null)
                        return Result.Failure("Control Account Manager user not found");

                    entity.AssignCAM(camUser.Id, userId);

                    // Update CAM assignment
                    var existingCAM = await _unitOfWork.Repository<ControlAccountAssignment>()
                        .GetAsync(
                            filter: a => a.ControlAccountId == id && a.Role == ControlAccountRole.Manager && a.IsActive,
                            cancellationToken: cancellationToken);

                    if (existingCAM != null && existingCAM.UserId != camUser.Id)
                    {
                        existingCAM.EndAssignment(userId);
                        _unitOfWork.Repository<ControlAccountAssignment>().Update(existingCAM);

                        var newAssignment = new ControlAccountAssignment(
                            entity.Id,
                            camUser.Id,
                            ControlAccountRole.Manager,
                            100);

                        newAssignment.CreatedBy = userId;
                        newAssignment.CreatedAt = DateTime.UtcNow;

                        await _unitOfWork.Repository<ControlAccountAssignment>().AddAsync(newAssignment, cancellationToken);
                    }
                }

                _unitOfWork.Repository<ControlAccount>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update Control Account: {ex.Message}");
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
                var entity = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == id && !ca.IsDeleted,
                        cancellationToken: cancellationToken);

                if (entity == null)
                    return Result.Failure("Control Account not found");

                entity.UpdateStatus(dto.NewStatus, userId);

                _unitOfWork.Repository<ControlAccount>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update Control Account status: {ex.Message}");
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
                var controlAccount = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == id && !ca.IsDeleted,
                        cancellationToken: cancellationToken);

                if (controlAccount == null)
                    return Result.Failure("Control Account not found");

                // Validate user exists
                var user = await _unitOfWork.Repository<User>()
                    .GetAsync(
                        filter: u => u.Id.ToString() == dto.UserId && !u.IsDeleted,
                        cancellationToken: cancellationToken);

                if (user == null)
                    return Result.Failure("User not found");

                // Check if assignment already exists
                var existingAssignment = await _unitOfWork.Repository<ControlAccountAssignment>()
                    .GetAsync(
                        filter: a => a.ControlAccountId == id && a.UserId == user.Id && a.IsActive,
                        cancellationToken: cancellationToken);

                if (existingAssignment != null)
                    return Result.Failure("User is already assigned to this Control Account");

                var assignment = new ControlAccountAssignment(
                    id,
                    user.Id,
                    dto.Role,
                    dto.AllocationPercentage);

                assignment.CreatedBy = userId;
                assignment.CreatedAt = DateTime.UtcNow;

                await _unitOfWork.Repository<ControlAccountAssignment>().AddAsync(assignment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to assign user to Control Account: {ex.Message}");
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
                var entity = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == id && !ca.IsDeleted,
                        cancellationToken: cancellationToken);

                if (entity == null)
                    return Result.Failure("Control Account not found");

                entity.UpdateProgress(percentComplete, userId);

                _unitOfWork.Repository<ControlAccount>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (ArgumentException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update Control Account progress: {ex.Message}");
            }
        }

        public async Task<Result> BaselineControlAccountAsync(
            Guid id,
            string userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == id && !ca.IsDeleted,
                        cancellationToken: cancellationToken);

                if (entity == null)
                    return Result.Failure("Control Account not found");

                entity.UpdateStatus(ControlAccountStatus.Baselined, userId);

                _unitOfWork.Repository<ControlAccount>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to baseline Control Account: {ex.Message}");
            }
        }

        public async Task<Result> CloseControlAccountAsync(
            Guid id,
            string userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == id && !ca.IsDeleted,
                        includeProperties: "WorkPackages",
                        cancellationToken: cancellationToken);

                if (entity == null)
                    return Result.Failure("Control Account not found");

                entity.Close(userId);

                _unitOfWork.Repository<ControlAccount>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to close Control Account: {ex.Message}");
            }
        }

        public async Task<Result> DeleteControlAccountAsync(
            Guid id,
            string userId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _unitOfWork.Repository<ControlAccount>()
                    .GetAsync(
                        filter: ca => ca.Id == id && !ca.IsDeleted,
                        includeProperties: "WorkPackages,PlanningPackages",
                        cancellationToken: cancellationToken);

                if (entity == null)
                    return Result.Failure("Control Account not found");

                entity.Delete(userId);

                _unitOfWork.Repository<ControlAccount>().Update(entity);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete Control Account: {ex.Message}");
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
                var user = await _unitOfWork.Repository<User>()
                    .GetAsync(
                        filter: u => u.Id.ToString() == userToRemove && !u.IsDeleted,
                        cancellationToken: cancellationToken);

                if (user == null)
                    return Result.Failure("User not found");

                var assignment = await _unitOfWork.Repository<ControlAccountAssignment>()
                    .GetAsync(
                        filter: a => a.ControlAccountId == id && a.UserId == user.Id && a.IsActive,
                        cancellationToken: cancellationToken);

                if (assignment == null)
                    return Result.Failure("User assignment not found");

                if (assignment.Role == ControlAccountRole.Manager)
                    return Result.Failure("Cannot remove the Control Account Manager. Please assign a new CAM first.");

                assignment.EndAssignment(userId);

                _unitOfWork.Repository<ControlAccountAssignment>().Update(assignment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to remove user from Control Account: {ex.Message}");
            }
        }

        private void MapComputedProperties(ControlAccountDto dto, ControlAccount entity)
        {
            dto.ProjectName = entity.Project?.Name ?? string.Empty;
            dto.PhaseName = entity.Phase?.Name ?? string.Empty;
            dto.CAMUserId = entity.CAMUserId.ToString();
            dto.CAMName = entity.CAMUser?.Name ?? string.Empty;
            dto.AC = entity.GetActualCost();
            dto.EV = entity.GetEarnedValue();
            dto.PV = entity.GetPlannedValue();
        }

        private void MapDetailProperties(ControlAccountDetailDto dto, ControlAccount entity)
        {
            MapComputedProperties(dto, entity);
            
            dto.WorkPackageCount = entity.WorkPackages?.Count(wp => !wp.IsDeleted) ?? 0;
            dto.PlanningPackageCount = entity.PlanningPackages?.Count(pp => !pp.IsDeleted) ?? 0;
            dto.ActualCost = entity.GetActualCost();
            dto.EarnedValue = entity.GetEarnedValue();
            dto.PlannedValue = entity.GetPlannedValue();
            dto.CPI = entity.GetCPI();
            dto.SPI = entity.GetSPI();

            if (entity.WorkPackages != null)
            {
                dto.WorkPackages = entity.WorkPackages
                    .Where(wp => !wp.IsDeleted && wp.IsWorkPackage() && wp.WorkPackageDetails != null)
                    .Select(wp => new WorkPackageSummaryDto
                    {
                        Id = wp.Id,
                        Code = wp.Code,
                        Name = wp.Name,
                        Budget = wp.WorkPackageDetails.Budget,
                        ActualCost = wp.WorkPackageDetails.ActualCost,
                        ProgressPercentage = wp.WorkPackageDetails.ProgressPercentage,
                        Status = wp.WorkPackageDetails.Status,
                        PlannedStartDate = wp.WorkPackageDetails.PlannedStartDate,
                        PlannedEndDate = wp.WorkPackageDetails.PlannedEndDate
                    })
                    .ToList();
            }

            if (entity.Assignments != null)
            {
                dto.Assignments = _mapper.Map<List<ControlAccountAssignmentDto>>(
                    entity.Assignments.Where(a => a.IsActive));
            }

            // Get latest EVM summary
            var latestEvm = entity.EVMRecords?.OrderByDescending(e => e.DataDate).FirstOrDefault();
            if (latestEvm != null)
            {
                dto.LatestEVM = _mapper.Map<EVMSummaryDto>(latestEvm);
            }
        }
    }
}