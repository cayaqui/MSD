using Application.Interfaces;
using Application.Interfaces.Progress;
using AutoMapper;
using Core.DTOs.Common;
using Core.DTOs.Progress.Milestones;
using Core.Enums.Projects;
using Domain.Entities.Progress;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Result = Core.Results.Result;
using MilestoneDto = Core.DTOs.Progress.Milestones.MilestoneDto;
using MilestoneFilterDto = Core.DTOs.Progress.Milestones.MilestoneFilterDto;
using UpdateMilestoneProgressDto = Core.DTOs.Progress.Milestones.UpdateMilestoneProgressDto;
using CompleteMilestoneDto = Core.DTOs.Progress.Milestones.CompleteMilestoneDto;
using ApproveMilestoneDto = Core.DTOs.Progress.Milestones.ApproveMilestoneDto;

namespace Application.Services.Progress;

public class MilestoneService : IMilestoneService
{
    private readonly IRepository<Milestone> _milestoneRepository;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public MilestoneService(
        IRepository<Milestone> milestoneRepository,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _milestoneRepository = milestoneRepository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<MilestoneDto>> GetMilestonesAsync(MilestoneFilterDto filter, CancellationToken cancellationToken = default)
    {
        var query = _milestoneRepository.GetAllQueryable()
            .Include(m => m.Project)
            .Include(m => m.Phase)
            .AsQueryable();

        if (filter.ProjectId.HasValue)
            query = query.Where(m => m.ProjectId == filter.ProjectId.Value);

        if (filter.PhaseId.HasValue)
            query = query.Where(m => m.PhaseId == filter.PhaseId.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query = query.Where(m => m.MilestoneCode.Contains(filter.SearchTerm) || 
                                    m.Name.Contains(filter.SearchTerm) ||
                                    (m.Description != null && m.Description.Contains(filter.SearchTerm)));

        if (filter.Type.HasValue)
            query = query.Where(m => m.Type == filter.Type.Value);

        if (filter.IsCritical.HasValue)
            query = query.Where(m => m.IsCritical == filter.IsCritical.Value);

        if (filter.IsContractual.HasValue)
            query = query.Where(m => m.IsContractual == filter.IsContractual.Value);

        if (filter.IsCompleted.HasValue)
            query = query.Where(m => m.IsCompleted == filter.IsCompleted.Value);

        if (filter.PlannedDateFrom.HasValue)
            query = query.Where(m => m.PlannedDate >= filter.PlannedDateFrom.Value);

        if (filter.PlannedDateTo.HasValue)
            query = query.Where(m => m.PlannedDate <= filter.PlannedDateTo.Value);

        if (filter.HasPayment.HasValue)
            query = query.Where(m => m.PaymentAmount.HasValue == filter.HasPayment.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = filter.SortBy?.ToLower() switch
        {
            "code" => filter.IsDescending ? query.OrderByDescending(m => m.MilestoneCode) : query.OrderBy(m => m.MilestoneCode),
            "name" => filter.IsDescending ? query.OrderByDescending(m => m.Name) : query.OrderBy(m => m.Name),
            "planneddate" => filter.IsDescending ? query.OrderByDescending(m => m.PlannedDate) : query.OrderBy(m => m.PlannedDate),
            "type" => filter.IsDescending ? query.OrderByDescending(m => m.Type) : query.OrderBy(m => m.Type),
            _ => filter.IsDescending ? query.OrderByDescending(m => m.PlannedDate) : query.OrderBy(m => m.PlannedDate)
        };

        // Apply pagination
        var milestones = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync(cancellationToken);

        var milestoneDtos = _mapper.Map<List<MilestoneDto>>(milestones);

        return new PagedResult<MilestoneDto>(
            milestoneDtos,
            totalCount,
            filter.PageNumber,
            filter.PageSize);
    }

    public async Task<MilestoneDto?> GetMilestoneByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var milestone = await _milestoneRepository.GetAllQueryable()
            .Include(m => m.Project)
            .Include(m => m.Phase)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        return milestone == null ? null : _mapper.Map<MilestoneDto>(milestone);
    }

    public async Task<List<MilestoneDto>> GetProjectMilestonesAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var milestones = await _milestoneRepository.GetAllQueryable()
            .Include(m => m.Project)
            .Include(m => m.Phase)
            .Where(m => m.ProjectId == projectId)
            .OrderBy(m => m.PlannedDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MilestoneDto>>(milestones);
    }

    public async Task<List<MilestoneDto>> GetUpcomingMilestonesAsync(Guid projectId, int days = 30, CancellationToken cancellationToken = default)
    {
        var cutoffDate = DateTime.UtcNow.AddDays(days);
        
        var milestones = await _milestoneRepository.GetAllQueryable()
            .Include(m => m.Project)
            .Include(m => m.Phase)
            .Where(m => m.ProjectId == projectId && 
                       !m.IsCompleted && 
                       m.PlannedDate <= cutoffDate)
            .OrderBy(m => m.PlannedDate)
            .ToListAsync(cancellationToken);

        return _mapper.Map<List<MilestoneDto>>(milestones);
    }

    public async Task<Core.Results.Result<Guid>> CreateMilestoneAsync(CreateMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validate milestone code is unique within project
            var codeExists = await _milestoneRepository.GetAllQueryable()
                .AnyAsync(m => m.ProjectId == dto.ProjectId && m.MilestoneCode == dto.MilestoneCode, cancellationToken);

            if (codeExists)
                return Result<Guid>.Failure("Milestone code already exists in this project");

            var milestone = new Milestone(
                dto.MilestoneCode,
                dto.Name,
                dto.ProjectId,
                dto.Type,
                dto.PlannedDate,
                dto.IsCritical,
                dto.IsContractual);

            if (!string.IsNullOrWhiteSpace(dto.Description))
            {
                var descProp = typeof(Milestone).GetProperty("Description");
                descProp?.SetValue(milestone, dto.Description);
            }

            if (dto.PhaseId.HasValue)
            {
                milestone.AssignToPhase(dto.PhaseId.Value);
            }

            if (dto.WorkPackageId.HasValue)
            {
                milestone.AssignToWorkPackage(dto.WorkPackageId.Value);
            }

            if (dto.PaymentAmount.HasValue && !string.IsNullOrWhiteSpace(dto.PaymentCurrency))
            {
                milestone.SetPaymentTerms(dto.PaymentAmount.Value, dto.PaymentCurrency);
            }

            if (!string.IsNullOrWhiteSpace(dto.CompletionCriteria))
            {
                milestone.SetCompletionCriteria(dto.CompletionCriteria);
            }

            if (!string.IsNullOrWhiteSpace(dto.AcceptanceCriteria))
            {
                milestone.SetAcceptanceCriteria(dto.AcceptanceCriteria);
            }

            await _milestoneRepository.AddAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(milestone.Id);
        }
        catch (Exception ex)
        {
            return Result<Guid>.Failure($"Failed to create milestone: {ex.Message}");
        }
    }

    public async Task<Result> UpdateMilestoneAsync(Guid id, UpdateMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            milestone.UpdateDetails(dto.Name, dto.Description, dto.IsCritical);

            if (dto.ForecastDate.HasValue)
            {
                milestone.UpdateSchedule(dto.ForecastDate);
            }

            if (dto.PhaseId.HasValue)
            {
                milestone.AssignToPhase(dto.PhaseId.Value);
            }

            if (dto.WorkPackageId.HasValue)
            {
                milestone.AssignToWorkPackage(dto.WorkPackageId.Value);
            }

            if (dto.PaymentAmount.HasValue && !string.IsNullOrWhiteSpace(dto.PaymentCurrency))
            {
                milestone.SetPaymentTerms(dto.PaymentAmount.Value, dto.PaymentCurrency);
            }

            if (!string.IsNullOrWhiteSpace(dto.CompletionCriteria))
            {
                milestone.SetCompletionCriteria(dto.CompletionCriteria);
            }

            if (!string.IsNullOrWhiteSpace(dto.AcceptanceCriteria))
            {
                milestone.SetAcceptanceCriteria(dto.AcceptanceCriteria);
            }

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update milestone: {ex.Message}");
        }
    }

    public async Task<Result> DeleteMilestoneAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            if (!await CanDeleteMilestoneAsync(id, cancellationToken))
                return Result.Failure("Cannot delete this milestone");

            // Soft delete
            milestone.IsDeleted = true;
            milestone.DeletedAt = DateTime.UtcNow;
            milestone.DeletedBy = userId.ToString();

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to delete milestone: {ex.Message}");
        }
    }

    public async Task<Result> UpdateMilestoneProgressAsync(Guid id, UpdateMilestoneProgressDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            milestone.UpdateProgress(dto.CompletionPercentage);

            if (dto.ForecastDate.HasValue)
            {
                milestone.UpdateSchedule(dto.ForecastDate);
            }

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to update milestone progress: {ex.Message}");
        }
    }

    public async Task<Result> CompleteMilestoneAsync(Guid id, CompleteMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            if (!await CanCompleteMilestoneAsync(id, cancellationToken))
                return Result.Failure("Cannot complete this milestone - check dependencies");

            milestone.Complete(dto.ActualDate);

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to complete milestone: {ex.Message}");
        }
    }

    public async Task<Result> ApproveMilestoneAsync(Guid id, ApproveMilestoneDto dto, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(id, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            if (!milestone.IsCompleted)
                return Result.Failure("Milestone must be completed before approval");

            milestone.Approve(userId.ToString());

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to approve milestone: {ex.Message}");
        }
    }

    public async Task<Result> SetPaymentTermsAsync(Guid milestoneId, decimal amount, string currency, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(milestoneId, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            milestone.SetPaymentTerms(amount, currency);

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(ex.Message);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to set payment terms: {ex.Message}");
        }
    }

    public async Task<Result> TriggerPaymentAsync(Guid milestoneId, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(milestoneId, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            if (!milestone.IsCompleted)
                return Result.Failure("Milestone must be completed before payment can be triggered");

            if (!milestone.IsApproved && milestone.RequiresApproval)
                return Result.Failure("Milestone must be approved before payment can be triggered");

            if (!milestone.PaymentAmount.HasValue)
                return Result.Failure("No payment amount set for this milestone");

            if (milestone.IsPaymentTriggered)
                return Result.Failure("Payment has already been triggered for this milestone");

            var paymentProp = typeof(Milestone).GetProperty("IsPaymentTriggered");
            paymentProp?.SetValue(milestone, true);

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to trigger payment: {ex.Message}");
        }
    }

    public async Task<PaymentSummaryDto> GetPaymentSummaryAsync(Guid projectId, string currency = "USD", CancellationToken cancellationToken = default)
    {
        var milestones = await _milestoneRepository.GetAllQueryable()
            .Where(m => m.ProjectId == projectId && 
                       m.PaymentAmount.HasValue && 
                       m.PaymentCurrency == currency)
            .ToListAsync(cancellationToken);

        var totalPayment = milestones.Sum(m => m.PaymentAmount ?? 0);
        var triggeredPayment = milestones.Where(m => m.IsPaymentTriggered).Sum(m => m.PaymentAmount ?? 0);
        var pendingPayment = totalPayment - triggeredPayment;

        return new PaymentSummaryDto
        {
            ProjectId = projectId,
            Currency = currency,
            TotalPaymentAmount = totalPayment,
            TriggeredPaymentAmount = triggeredPayment,
            PendingPaymentAmount = pendingPayment,
            PaymentProgress = totalPayment > 0 ? (triggeredPayment / totalPayment * 100) : 0
        };
    }

    public async Task<MilestoneDashboardDto> GetMilestoneDashboardAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var milestones = await _milestoneRepository.GetAllQueryable()
            .Where(m => m.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        var today = DateTime.UtcNow.Date;
        var thirtyDaysFromNow = today.AddDays(30);

        var dashboard = new MilestoneDashboardDto
        {
            ProjectId = projectId,
            TotalMilestones = milestones.Count,
            CompletedMilestones = milestones.Count(m => m.IsCompleted),
            UpcomingMilestones = milestones.Count(m => !m.IsCompleted && m.PlannedDate <= thirtyDaysFromNow),
            OverdueMilestones = milestones.Count(m => !m.IsCompleted && m.PlannedDate < today),
            CriticalMilestones = milestones.Count(m => m.IsCritical),
            ContractualMilestones = milestones.Count(m => m.IsContractual),
            CompletionPercentage = milestones.Any() ? 
                (decimal)milestones.Count(m => m.IsCompleted) / milestones.Count * 100 : 0,
            NextMilestone = _mapper.Map<MilestoneDto>(
                milestones.Where(m => !m.IsCompleted)
                          .OrderBy(m => m.PlannedDate)
                          .FirstOrDefault()),
            RecentlyCompleted = _mapper.Map<List<MilestoneDto>>(
                milestones.Where(m => m.IsCompleted && m.ActualDate >= today.AddDays(-30))
                          .OrderByDescending(m => m.ActualDate)
                          .Take(5)
                          .ToList()),
            UpcomingList = _mapper.Map<List<MilestoneDto>>(
                milestones.Where(m => !m.IsCompleted && m.PlannedDate <= thirtyDaysFromNow)
                          .OrderBy(m => m.PlannedDate)
                          .Take(10)
                          .ToList())
        };

        return dashboard;
    }

    public async Task<Result> SetMilestoneDependenciesAsync(Guid milestoneId, string[]? predecessors, string[]? successors, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var milestone = await _milestoneRepository.GetByIdAsync(milestoneId, cancellationToken);
            if (milestone == null)
                return Result.Failure("Milestone not found");

            // Validate dependencies
            var validationErrors = await ValidateDependenciesAsync(milestoneId, predecessors ?? Array.Empty<string>(), successors ?? Array.Empty<string>(), cancellationToken);
            if (validationErrors.Any())
                return Result.Failure($"Invalid dependencies: {string.Join(", ", validationErrors)}");

            var predProp = typeof(Milestone).GetProperty("PredecessorMilestones");
            predProp?.SetValue(milestone, predecessors != null ? JsonSerializer.Serialize(predecessors) : null);

            var succProp = typeof(Milestone).GetProperty("SuccessorMilestones");
            succProp?.SetValue(milestone, successors != null ? JsonSerializer.Serialize(successors) : null);

            await _milestoneRepository.UpdateAsync(milestone, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Failed to set milestone dependencies: {ex.Message}");
        }
    }

    public async Task<List<string>> ValidateDependenciesAsync(Guid milestoneId, string[] predecessors, string[] successors, CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();

        var milestone = await _milestoneRepository.GetByIdAsync(milestoneId, cancellationToken);
        if (milestone == null)
        {
            errors.Add("Milestone not found");
            return errors;
        }

        // Check if predecessors exist
        foreach (var pred in predecessors)
        {
            var exists = await _milestoneRepository.GetAllQueryable()
                .AnyAsync(m => m.ProjectId == milestone.ProjectId && m.MilestoneCode == pred, cancellationToken);

            if (!exists)
                errors.Add($"Predecessor milestone '{pred}' not found");
        }

        // Check if successors exist
        foreach (var succ in successors)
        {
            var exists = await _milestoneRepository.GetAllQueryable()
                .AnyAsync(m => m.ProjectId == milestone.ProjectId && m.MilestoneCode == succ, cancellationToken);

            if (!exists)
                errors.Add($"Successor milestone '{succ}' not found");
        }

        // Check for circular dependencies
        if (predecessors.Contains(milestone.MilestoneCode) || successors.Contains(milestone.MilestoneCode))
            errors.Add("Milestone cannot be its own predecessor or successor");

        return errors;
    }

    public async Task<bool> ValidateMilestoneCodeAsync(string code, Guid projectId, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _milestoneRepository.GetAllQueryable()
            .Where(m => m.ProjectId == projectId && m.MilestoneCode == code);

        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId.Value);

        return !await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> CanDeleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var milestone = await _milestoneRepository.GetByIdAsync(id, cancellationToken);
        if (milestone == null)
            return false;

        // Cannot delete completed milestones
        if (milestone.IsCompleted)
            return false;

        // Cannot delete if payment has been triggered
        if (milestone.IsPaymentTriggered)
            return false;

        // Cannot delete if has successors
        if (!string.IsNullOrWhiteSpace(milestone.SuccessorMilestones))
        {
            var successors = JsonSerializer.Deserialize<string[]>(milestone.SuccessorMilestones);
            if (successors?.Any() == true)
                return false;
        }

        return true;
    }

    public async Task<bool> CanCompleteMilestoneAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var milestone = await _milestoneRepository.GetByIdAsync(id, cancellationToken);
        if (milestone == null)
            return false;

        // Already completed
        if (milestone.IsCompleted)
            return false;

        // Check if all predecessor milestones are completed
        if (!string.IsNullOrWhiteSpace(milestone.PredecessorMilestones))
        {
            var predecessors = JsonSerializer.Deserialize<string[]>(milestone.PredecessorMilestones);
            if (predecessors?.Any() == true)
            {
                var incompletePredsExist = await _milestoneRepository.GetAllQueryable()
                    .AnyAsync(m => m.ProjectId == milestone.ProjectId && 
                                  predecessors.Contains(m.MilestoneCode) && 
                                  !m.IsCompleted, cancellationToken);

                if (incompletePredsExist)
                    return false;
            }
        }

        return true;
    }
}