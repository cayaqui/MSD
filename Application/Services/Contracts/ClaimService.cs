using Application.Interfaces.Contracts;
using Application.Services.Base;
using Core.DTOs.Contracts.ChangeOrders;
using Core.DTOs.Contracts.Claims;
using Core.Enums.Contracts;
using Domain.Entities.Contracts.Core;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Application.Services.Contracts;

public class ClaimService : BaseService<Claim, ClaimDto, CreateClaimDto, UpdateClaimDto>, IClaimService
{
    public ClaimService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<ClaimService> logger)
        : base(unitOfWork, mapper, logger)
    {
    }

    // Override GetByIdAsync to include related data
    public override async Task<ClaimDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await Repository.GetAsync(
            c => c.Id == id,
            includeProperties: "Contract,Documents",
            cancellationToken: cancellationToken);

        if (claim == null)
            throw new NotFoundException(nameof(Claim), id);

        return Mapper.Map<ClaimDto>(claim);
    }

    // Override CreateAsync to handle claim-specific logic
    public override async Task<ClaimDto> CreateAsync(CreateClaimDto createDto, CancellationToken cancellationToken = default)
    {
        // Check if claim number already exists for this contract
        var existingClaim = await Repository.GetAsync(
            c => c.ContractId == createDto.ContractId && c.ClaimNumber == createDto.ClaimNumber,
            cancellationToken: cancellationToken);

        if (existingClaim != null)
            throw new ConflictException($"Claim number {createDto.ClaimNumber} already exists for this contract");

        var claim = Mapper.Map<Claim>(createDto);
        
        // Set audit fields
        var currentUserId = await GetCurrentUserIdAsync();
        claim.CreatedBy = currentUserId;

        await Repository.AddAsync(claim, cancellationToken);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return await GetByIdAsync(claim.Id, cancellationToken);
    }

    // Override UpdateAsync to handle claim-specific logic
    public override async Task<ClaimDto> UpdateAsync(Guid id, UpdateClaimDto updateDto, CancellationToken cancellationToken = default)
    {
        // Get existing claim
        var claim = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (claim == null)
            throw new NotFoundException(nameof(Claim), id);

        // Map update
        Mapper.Map(updateDto, claim);
        
        // Update entity
        Repository.Update(claim);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        // Return mapped DTO
        return Mapper.Map<ClaimDto>(claim);
    }

    // Claim queries
    public async Task<IEnumerable<ClaimDto>> GetByContractAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var claims = await Repository.GetAllAsync(
            filter: c => c.ContractId == contractId,
            orderBy: q => q.OrderByDescending(c => c.SubmissionDate),
            includeProperties: "Contract,Documents",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetByStatusAsync(ClaimStatus status, CancellationToken cancellationToken = default)
    {
        var claims = await Repository.GetAllAsync(
            filter: c => c.Status == status,
            includeProperties: "Contract",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<IEnumerable<ClaimDto>> GetByTypeAsync(ClaimType type, CancellationToken cancellationToken = default)
    {
        var claims = await Repository.GetAllAsync(
            filter: c => c.Type == type,
            includeProperties: "Contract",
            cancellationToken: cancellationToken);

        return Mapper.Map<IEnumerable<ClaimDto>>(claims);
    }

    public async Task<PagedResult<ClaimDto>> GetPagedAsync(ClaimQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        // Build filter predicate
        Expression<Func<Claim, bool>>? predicate = null;
        
        if (!string.IsNullOrEmpty(parameters.SearchTerm) || parameters.ContractId.HasValue || 
            parameters.Status.HasValue || parameters.Type.HasValue)
        {
            predicate = c => 
                (string.IsNullOrEmpty(parameters.SearchTerm) || 
                 c.ClaimNumber.Contains(parameters.SearchTerm) ||
                 c.Title.Contains(parameters.SearchTerm) ||
                 c.Description.Contains(parameters.SearchTerm)) &&
                (!parameters.ContractId.HasValue || c.ContractId == parameters.ContractId.Value) &&
                (!parameters.Status.HasValue || c.Status == parameters.Status.Value) &&
                (!parameters.Type.HasValue || c.Type == parameters.Type.Value);
        }

        var pagedResult = await Repository.GetPagedAsync(
            pageNumber: parameters.PageNumber,
            pageSize: parameters.PageSize,
            filter: predicate,
            orderBy: GetOrderBy(parameters.SortBy, parameters.IsAscending),
            includeProperties: "Contract");

        var dtos = Mapper.Map<IEnumerable<ClaimDto>>(pagedResult.Items);

        return new PagedResult<ClaimDto>(
            dtos,
            pagedResult.TotalCount,
            parameters.PageNumber,
            parameters.PageSize);
    }

    // Claim workflow methods
    public async Task<ClaimDto> SubmitAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var claim = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (claim == null)
            throw new NotFoundException(nameof(Claim), id);

        claim.Submit();
        
        Repository.Update(claim);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ClaimDto>(claim);
    }

    public async Task<ClaimDto> AssessAsync(Guid id, AssessClaimDto assessDto, CancellationToken cancellationToken = default)
    {
        var claim = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (claim == null)
            throw new NotFoundException(nameof(Claim), id);

        claim.Assess(
            assessDto.AssessedAmount,
            assessDto.AssessedTimeExtension,
            assessDto.HasMerit,
            assessDto.LiabilityPercentage,
            assessDto.AssessmentComments);
        
        Repository.Update(claim);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ClaimDto>(claim);
    }

    public async Task<ClaimDto> ResolveAsync(Guid id, ResolveClaimDto resolveDto, CancellationToken cancellationToken = default)
    {
        var claim = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (claim == null)
            throw new NotFoundException(nameof(Claim), id);

        claim.Resolve(
            resolveDto.Resolution,
            resolveDto.ApprovedAmount,
            resolveDto.ApprovedTimeExtension,
            resolveDto.ResolutionDetails,
            resolveDto.SettlementTerms);
        
        Repository.Update(claim);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ClaimDto>(claim);
    }

    public async Task<ClaimDto> RecordPaymentAsync(Guid id, RecordClaimPaymentDto paymentDto, CancellationToken cancellationToken = default)
    {
        var claim = await Repository.GetAsync(
            c => c.Id == id,
            tracked: true,
            cancellationToken: cancellationToken);

        if (claim == null)
            throw new NotFoundException(nameof(Claim), id);

        claim.RecordPayment(paymentDto.Amount);
        
        Repository.Update(claim);
        await UnitOfWork.SaveChangesAsync(cancellationToken);

        return Mapper.Map<ClaimDto>(claim);
    }

    // Financial summary methods
    public async Task<ClaimFinancialSummary> GetFinancialSummaryAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var claims = await Repository.GetAllAsync(
            filter: c => c.ContractId == contractId,
            cancellationToken: cancellationToken);

        var claimsList = claims.ToList();

        return new ClaimFinancialSummary
        {
            ContractId = contractId,
            TotalClaimed = claimsList.Sum(c => c.ClaimedAmount),
            TotalAssessed = claimsList.Sum(c => c.AssessedAmount),
            TotalApproved = claimsList.Sum(c => c.ApprovedAmount),
            TotalPaid = claimsList.Sum(c => c.PaidAmount),
            ContractorClaims = claimsList.Where(c => c.Direction == ClaimDirection.ContractorClaim).Sum(c => c.ClaimedAmount),
            EmployerClaims = claimsList.Where(c => c.Direction == ClaimDirection.EmployerClaim).Sum(c => c.ClaimedAmount),
            NetPosition = claimsList.Where(c => c.Direction == ClaimDirection.ContractorClaim).Sum(c => c.ApprovedAmount) -
                         claimsList.Where(c => c.Direction == ClaimDirection.EmployerClaim).Sum(c => c.ApprovedAmount)
        };
    }

    // Analytics methods
    public async Task<Dictionary<ClaimType, int>> GetClaimsByTypeAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var claims = await Repository.GetAllAsync(
            filter: c => c.ContractId == contractId,
            cancellationToken: cancellationToken);

        return claims
            .GroupBy(c => c.Type)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<Dictionary<ClaimStatus, int>> GetClaimsByStatusAsync(Guid contractId, CancellationToken cancellationToken = default)
    {
        var claims = await Repository.GetAllAsync(
            filter: c => c.ContractId == contractId,
            cancellationToken: cancellationToken);

        return claims
            .GroupBy(c => c.Status)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    public async Task<IEnumerable<ClaimTimelineEntry>> GetTimelineAsync(Guid claimId, CancellationToken cancellationToken = default)
    {
        var claim = await Repository.GetAsync(
            c => c.Id == claimId,
            includeProperties: "Contract",
            cancellationToken: cancellationToken);

        if (claim == null)
            throw new NotFoundException(nameof(Claim), claimId);

        var timeline = new List<ClaimTimelineEntry>();

        // Add key events to timeline
        timeline.Add(new ClaimTimelineEntry
        {
            Date = claim.EventDate,
            Event = "Event Occurred",
            Description = "The event giving rise to the claim occurred"
        });

        timeline.Add(new ClaimTimelineEntry
        {
            Date = claim.NotificationDate,
            Event = "Notification",
            Description = "Formal notification of claim"
        });

        timeline.Add(new ClaimTimelineEntry
        {
            Date = claim.SubmissionDate,
            Event = "Submission",
            Description = "Claim submitted for assessment"
        });

        if (claim.ActualResponseDate.HasValue)
        {
            timeline.Add(new ClaimTimelineEntry
            {
                Date = claim.ActualResponseDate.Value,
                Event = "Response Received",
                Description = "Response to claim received"
            });
        }

        if (claim.ResolutionDate.HasValue)
        {
            timeline.Add(new ClaimTimelineEntry
            {
                Date = claim.ResolutionDate.Value,
                Event = "Resolution",
                Description = $"Claim resolved: {claim.Resolution}"
            });
        }

        return timeline.OrderBy(t => t.Date);
    }

    // Private helper methods
    private Func<IQueryable<Claim>, IOrderedQueryable<Claim>> GetOrderBy(string? sortBy, bool isAscending)
    {
        Expression<Func<Claim, object>> orderExpression = sortBy?.ToLower() switch
        {
            "number" => c => c.ClaimNumber,
            "title" => c => c.Title,
            "amount" => c => c.ClaimedAmount,
            "status" => c => c.Status,
            "date" => c => c.SubmissionDate,
            _ => c => c.CreatedAt
        };

        return isAscending
            ? q => q.OrderBy(orderExpression)
            : q => q.OrderByDescending(orderExpression);
    }

    private async Task<string> GetCurrentUserIdAsync()
    {
        // This should be injected from ICurrentUserService
        return "System";
    }
}