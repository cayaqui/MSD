using Application.Interfaces.Cost;
using Core.DTOs.Cost.Commitments;
using Core.DTOs.Cost.CommitmentItems;
using Core.DTOs.Cost.CommitmentWorkPackage;
using Domain.Entities.Cost.Commitments;
using Core.Enums.Cost;

using System.Text;

namespace Application.Services.Cost
{
    /// <summary>
    /// Service implementation for Commitment management
    /// </summary>
    public class CommitmentService : ICommitmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public CommitmentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResult<CommitmentListDto>> GetCommitmentsAsync(CommitmentFilterDto filter)
        {
            var query = _unitOfWork.Repository<Commitment>().Query();

            // Apply filters
            if (filter.ProjectId.HasValue)
                query = query.Where(c => c.ProjectId == filter.ProjectId.Value);

            if (filter.ContractorId.HasValue)
                query = query.Where(c => c.ContractorId == filter.ContractorId.Value);

            if (filter.ControlAccountId.HasValue)
                query = query.Where(c => c.ControlAccountId == filter.ControlAccountId.Value);

            if (filter.BudgetItemId.HasValue)
                query = query.Where(c => c.BudgetItemId == filter.BudgetItemId.Value);

            if (filter.Type.HasValue)
                query = query.Where(c => c.Type == filter.Type.Value);

            if (filter.Status.HasValue)
                query = query.Where(c => c.Status == filter.Status.Value);

            // Search text
            if (!string.IsNullOrWhiteSpace(filter.SearchText))
            {
                var searchText = filter.SearchText.ToLower();
                query = query.Where(c =>
                    c.CommitmentNumber.ToLower().Contains(searchText) ||
                    c.Title.ToLower().Contains(searchText) ||
                    (c.PurchaseOrderNumber != null && c.PurchaseOrderNumber.ToLower().Contains(searchText)) ||
                    (c.ContractNumber != null && c.ContractNumber.ToLower().Contains(searchText)));
            }

            // Specific searches
            if (!string.IsNullOrWhiteSpace(filter.CommitmentNumber))
                query = query.Where(c => c.CommitmentNumber == filter.CommitmentNumber);

            if (!string.IsNullOrWhiteSpace(filter.ContractNumber))
                query = query.Where(c => c.ContractNumber == filter.ContractNumber);

            if (!string.IsNullOrWhiteSpace(filter.PurchaseOrderNumber))
                query = query.Where(c => c.PurchaseOrderNumber == filter.PurchaseOrderNumber);

            // Date range
            if (filter.DateFrom.HasValue)
                query = query.Where(c => c.ContractDate >= filter.DateFrom.Value);

            if (filter.DateTo.HasValue)
                query = query.Where(c => c.ContractDate <= filter.DateTo.Value);

            // Amount range
            if (filter.AmountMin.HasValue)
                query = query.Where(c => c.CommittedAmount >= filter.AmountMin.Value);

            if (filter.AmountMax.HasValue)
                query = query.Where(c => c.CommittedAmount <= filter.AmountMax.Value);

            // Status filters
            if (filter.IsActive.HasValue)
                query = filter.IsActive.Value ? 
                    query.Where(c => !c.IsDeleted) : 
                    query.Where(c => c.IsDeleted);

            if (filter.IsOverCommitted.HasValue && filter.IsOverCommitted.Value)
                query = query.Where(c => c.InvoicedAmount > c.CommittedAmount);

            if (filter.IsExpired.HasValue && filter.IsExpired.Value)
                query = query.Where(c => DateTime.UtcNow > c.EndDate && c.Status == CommitmentStatus.Active);

            // Sorting
            query = filter.SortBy?.ToLower() switch
            {
                "title" => filter.SortDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
                "contractdate" => filter.SortDescending ? query.OrderByDescending(c => c.ContractDate) : query.OrderBy(c => c.ContractDate),
                "amount" => filter.SortDescending ? query.OrderByDescending(c => c.CommittedAmount) : query.OrderBy(c => c.CommittedAmount),
                "status" => filter.SortDescending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),
                _ => filter.SortDescending ? query.OrderByDescending(c => c.CommitmentNumber) : query.OrderBy(c => c.CommitmentNumber)
            };

            var totalItems = await query.CountAsync();

            var items = await query
                .Include(c => c.Contractor)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();

            var dtos = _mapper.Map<List<CommitmentListDto>>(items);

            return new PagedResult<CommitmentListDto>
            {
                Items = dtos,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)filter.PageSize)
            };
        }

        public async Task<CommitmentDetailDto?> GetCommitmentDetailAsync(Guid commitmentId)
        {
            var entity = await _unitOfWork.Repository<Commitment>()
                .GetAsync(
                    filter: c => c.Id == commitmentId && !c.IsDeleted,
                    includeProperties: "Project,BudgetItem,Contractor,ControlAccount,Items,WorkPackageAllocations.WBSElement,Revisions,Invoices");

            if (entity == null)
                return null;

            var dto = _mapper.Map<CommitmentDetailDto>(entity);
            
            // Map financial summary
            dto.FinancialSummary = new CommitmentFinancialSummary
            {
                OriginalBudget = entity.OriginalAmount,
                CurrentBudget = entity.RevisedAmount,
                TotalCommitted = entity.CommittedAmount,
                TotalInvoiced = entity.InvoicedAmount,
                TotalPaid = entity.PaidAmount,
                TotalRetention = entity.RetentionAmount,
                TotalOutstanding = entity.InvoicedAmount - entity.PaidAmount,
                BudgetVariance = entity.RevisedAmount - entity.OriginalAmount,
                BudgetVariancePercentage = entity.OriginalAmount > 0 ? 
                    ((entity.RevisedAmount - entity.OriginalAmount) / entity.OriginalAmount) * 100 : 0,
                TotalWorkPackages = entity.WorkPackageAllocations?.Count ?? 0,
                ActiveWorkPackages = entity.WorkPackageAllocations?.Count(w => w.AllocatedAmount > 0) ?? 0,
                AverageWorkPackageUtilization = entity.WorkPackageAllocations?.Any() == true ?
                    entity.WorkPackageAllocations.Average(w => w.InvoicedAmount / Math.Max(w.AllocatedAmount, 1)) * 100 : 0
            };

            // Map performance metrics
            dto.PerformanceMetrics = new CommitmentPerformanceMetrics
            {
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                InvoicingEfficiency = entity.CommittedAmount > 0 ? 
                    (entity.InvoicedAmount / entity.CommittedAmount) * 100 : 0,
                PaymentEfficiency = entity.InvoicedAmount > 0 ? 
                    (entity.PaidAmount / entity.InvoicedAmount) * 100 : 0,
                CostPerformanceIndex = 1.0m, // Would need actual earned value data
                TotalInvoices = entity.Invoices?.Count ?? 0,
                ApprovedInvoices = entity.Invoices?.Count(i => i.Status == InvoiceStatus.Approved) ?? 0,
                RejectedInvoices = entity.Invoices?.Count(i => i.Status == InvoiceStatus.Rejected) ?? 0,
                InvoiceApprovalRate = entity.Invoices?.Any() == true ?
                    (entity.Invoices.Count(i => i.Status == InvoiceStatus.Approved) / (decimal)entity.Invoices.Count) * 100 : 0,
                IsDelayed = DateTime.UtcNow > entity.EndDate && entity.Status == CommitmentStatus.Active,
                IsBudgetExceeded = entity.IsOverCommitted(),
                RiskScore = CalculateRiskScore(entity)
            };

            // Map invoices if loaded
            if (entity.Invoices != null)
            {
                dto.Invoices = entity.Invoices.Select(i => new CommitmentInvoiceDto
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    VendorInvoiceNumber = i.VendorInvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    ReceivedDate = i.ReceivedDate,
                    DueDate = i.DueDate,
                    GrossAmount = i.GrossAmount,
                    TaxAmount = i.TaxAmount,
                    NetAmount = i.NetAmount,
                    RetentionAmount = i.RetentionAmount,
                    TotalAmount = i.TotalAmount,
                    PaidAmount = i.PaidAmount,
                    Currency = i.Currency,
                    Status = i.Status.ToString(),
                    ApprovedDate = i.ApprovedDate,
                    PaidDate = i.PaidDate
                }).ToList();
            }

            // For now, leave audit trail empty - would need audit tracking implementation
            dto.AuditTrail = new List<CommitmentAuditDto>();

            return dto;
        }

        public async Task<CommitmentDto?> GetCommitmentAsync(Guid commitmentId)
        {
            var entity = await _unitOfWork.Repository<Commitment>()
                .GetAsync(
                    filter: c => c.Id == commitmentId && !c.IsDeleted,
                    includeProperties: "Project,BudgetItem,Contractor,ControlAccount");

            if (entity == null)
                return null;

            return _mapper.Map<CommitmentDto>(entity);
        }

        public async Task<List<CommitmentListDto>> GetProjectCommitmentsAsync(Guid projectId)
        {
            var entities = await _unitOfWork.Repository<Commitment>()
                .GetAllAsync(
                    filter: c => c.ProjectId == projectId && !c.IsDeleted,
                    includeProperties: "Contractor");

            return _mapper.Map<List<CommitmentListDto>>(entities);
        }

        public async Task<CommitmentSummaryDto> GetProjectCommitmentSummaryAsync(Guid projectId)
        {
            var commitments = await _unitOfWork.Repository<Commitment>()
                .GetAllAsync(filter: c => c.ProjectId == projectId && !c.IsDeleted);

            var summary = new CommitmentSummaryDto
            {
                ProjectId = projectId,
                TotalCommitments = commitments.Count(),
                DraftCount = commitments.Count(c => c.Status == CommitmentStatus.Draft),
                ActiveCount = commitments.Count(c => c.Status == CommitmentStatus.Active),
                ClosedCount = commitments.Count(c => c.Status == CommitmentStatus.Closed),
                TotalOriginalAmount = commitments.Sum(c => c.OriginalAmount),
                TotalRevisedAmount = commitments.Sum(c => c.RevisedAmount),
                TotalCommittedAmount = commitments.Sum(c => c.CommittedAmount),
                TotalInvoicedAmount = commitments.Sum(c => c.InvoicedAmount),
                TotalPaidAmount = commitments.Sum(c => c.PaidAmount),
                TotalRetentionAmount = commitments.Sum(c => c.RetentionAmount),
                OverCommittedCount = commitments.Count(c => c.IsOverCommitted()),
                ExpiredCount = commitments.Count(c => c.IsExpired())
            };

            // Calculate averages
            if (summary.TotalCommitments > 0)
            {
                summary.AverageInvoicedPercentage = commitments.Average(c => c.GetInvoicedPercentage());
                summary.AveragePaidPercentage = commitments.Average(c => c.GetPaidPercentage());
            }

            // Group by type
            summary.CountByType = commitments
                .GroupBy(c => c.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count());

            summary.AmountByType = commitments
                .GroupBy(c => c.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Sum(c => c.CommittedAmount));

            return summary;
        }

        public async Task<CommitmentDto> CreateCommitmentAsync(CreateCommitmentDto dto)
        {
            // Validate commitment number uniqueness
            var existingCommitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.CommitmentNumber == dto.CommitmentNumber && !c.IsDeleted);

            if (existingCommitment != null)
                throw new InvalidOperationException($"Commitment number {dto.CommitmentNumber} already exists");

            // Create commitment
            var commitment = new Commitment(
                dto.ProjectId,
                dto.CommitmentNumber,
                dto.Title,
                dto.Type,
                dto.OriginalAmount,
                dto.Currency,
                dto.ContractDate,
                dto.StartDate,
                dto.EndDate);

            // Set optional properties
            if (!string.IsNullOrWhiteSpace(dto.Description))
                commitment.UpdateDetails(dto.Title, dto.Description, dto.ScopeOfWork, dto.Deliverables);

            if (dto.ContractorId.HasValue)
                commitment.AssignToContractor(dto.ContractorId.Value);

            if (dto.BudgetItemId.HasValue)
                commitment.AssignToBudget(dto.BudgetItemId.Value);

            if (dto.ControlAccountId.HasValue)
                commitment.AssignToControlAccount(dto.ControlAccountId.Value);

            commitment.SetContractReferences(
                dto.PurchaseOrderNumber,
                dto.ContractNumber,
                dto.VendorReference,
                dto.AccountingReference);

            commitment.SetPaymentTerms(
                dto.PaymentTermsDays,
                dto.RetentionPercentage,
                dto.AdvancePaymentAmount);

            commitment.SetContractType(dto.IsFixedPrice, dto.IsTimeAndMaterial);

            var currentUser = _currentUserService.UserId;
            commitment.CreatedBy = currentUser;

            await _unitOfWork.Repository<Commitment>().AddAsync(commitment);

            // Add work package allocations if provided
            if (dto.WorkPackageAllocations != null && dto.WorkPackageAllocations.Any())
            {
                foreach (var allocation in dto.WorkPackageAllocations)
                {
                    commitment.AddWorkPackageAllocation(allocation.WBSElementId, allocation.AllocatedAmount);
                }
            }

            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve created commitment");
        }

        public async Task<CommitmentDto> UpdateCommitmentAsync(Guid commitmentId, UpdateCommitmentDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            // Update basic details
            if (!string.IsNullOrWhiteSpace(dto.Title))
            {
                commitment.UpdateDetails(
                    dto.Title,
                    dto.Description,
                    dto.ScopeOfWork,
                    dto.Deliverables);
            }

            // Update assignments
            if (dto.ContractorId.HasValue)
                commitment.AssignToContractor(dto.ContractorId.Value);

            if (dto.BudgetItemId.HasValue)
                commitment.AssignToBudget(dto.BudgetItemId.Value);

            if (dto.ControlAccountId.HasValue)
                commitment.AssignToControlAccount(dto.ControlAccountId.Value);

            // Update references
            commitment.SetContractReferences(
                dto.PurchaseOrderNumber,
                dto.ContractNumber,
                dto.VendorReference,
                dto.AccountingReference);

            // Update payment terms
            if (dto.PaymentTermsDays.HasValue || dto.RetentionPercentage.HasValue || dto.AdvancePaymentAmount.HasValue)
            {
                commitment.SetPaymentTerms(
                    dto.PaymentTermsDays,
                    dto.RetentionPercentage,
                    dto.AdvancePaymentAmount);
            }

            // Update contract type
            if (dto.IsFixedPrice.HasValue && dto.IsTimeAndMaterial.HasValue)
            {
                commitment.SetContractType(dto.IsFixedPrice.Value, dto.IsTimeAndMaterial.Value);
            }

            // Update performance
            if (dto.PerformancePercentage.HasValue)
            {
                commitment.UpdatePerformance(dto.PerformancePercentage.Value, dto.ExpectedCompletionDate);
            }

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve updated commitment");
        }

        public async Task<bool> DeleteCommitmentAsync(Guid commitmentId)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                return false;

            if (commitment.Status != CommitmentStatus.Draft)
                throw new InvalidOperationException("Only draft commitments can be deleted");

            if (commitment.InvoicedAmount > 0)
                throw new InvalidOperationException("Cannot delete commitment with invoiced amounts");

            commitment.IsDeleted = true;
            commitment.DeletedAt = DateTime.UtcNow;
            commitment.DeletedBy = _currentUserService.UserId;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<CommitmentDto> SubmitForApprovalAsync(Guid commitmentId)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.SubmitForApproval();

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> ApproveCommitmentAsync(Guid commitmentId, ApproveCommitmentDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            var currentUser = _currentUserService.UserId;
            commitment.Approve(currentUser, dto.ApprovalNotes);
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> RejectCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.Cancel(dto.Reason);

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> ActivateCommitmentAsync(Guid commitmentId)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.Activate();

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> ReviseCommitmentAsync(Guid commitmentId, ReviseCommitmentDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(
                    filter: c => c.Id == commitmentId && !c.IsDeleted,
                    includeProperties: "Revisions");

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.Revise(dto.RevisedAmount, dto.Reason);

            // Set revision number
            var revisionNumber = commitment.Revisions.Count;
            var revision = commitment.Revisions.Last();
            revision.SetRevisionNumber(revisionNumber);

            if (!string.IsNullOrWhiteSpace(dto.ChangeOrderReference))
                revision.SetChangeOrderReference(dto.ChangeOrderReference);

            var currentUser = _currentUserService.UserId;
            revision.SetApproval(currentUser);
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> CloseCommitmentAsync(Guid commitmentId)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.Close();

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> CancelCommitmentAsync(Guid commitmentId, CancelCommitmentDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.Cancel(dto.Reason);

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> AddWorkPackageAllocationAsync(Guid commitmentId, CommitmentWorkPackageAllocationDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(
                    filter: c => c.Id == commitmentId && !c.IsDeleted,
                    includeProperties: "WorkPackageAllocations");

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.AddWorkPackageAllocation(dto.WBSElementId, dto.AllocatedAmount);

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> UpdateWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId, decimal newAmount)
        {
            var allocation = await _unitOfWork.Repository<CommitmentWorkPackage>()
                .GetAsync(filter: a => a.Id == allocationId && a.CommitmentId == commitmentId);

            if (allocation == null)
                throw new InvalidOperationException("Work package allocation not found");

            var currentUser = _currentUserService.UserId;
            allocation.UpdateAllocation(newAmount, currentUser);

            _unitOfWork.Repository<CommitmentWorkPackage>().Update(allocation);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitmentId) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<bool> RemoveWorkPackageAllocationAsync(Guid commitmentId, Guid allocationId)
        {
            var allocation = await _unitOfWork.Repository<CommitmentWorkPackage>()
                .GetAsync(filter: a => a.Id == allocationId && a.CommitmentId == commitmentId);

            if (allocation == null)
                return false;

            if (allocation.InvoicedAmount > 0)
                throw new InvalidOperationException("Cannot remove allocation with invoiced amounts");

            _unitOfWork.Repository<CommitmentWorkPackage>().Remove(allocation);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<CommitmentWorkPackageDto>> GetWorkPackageAllocationsAsync(Guid commitmentId)
        {
            var allocations = await _unitOfWork.Repository<CommitmentWorkPackage>()
                .GetAllAsync(
                    filter: a => a.CommitmentId == commitmentId,
                    includeProperties: "WBSElement");

            return _mapper.Map<List<CommitmentWorkPackageDto>>(allocations);
        }

        public async Task<CommitmentDto> AddCommitmentItemAsync(Guid commitmentId, CreateCommitmentItemDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(
                    filter: c => c.Id == commitmentId && !c.IsDeleted,
                    includeProperties: "Items");

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.AddItem(
                dto.ItemNumber,
                dto.ItemCode,
                dto.Description,
                dto.Quantity,
                dto.UnitOfMeasure,
                dto.UnitPrice);

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> UpdateCommitmentItemAsync(Guid commitmentId, Guid itemId, UpdateCommitmentItemDto dto)
        {
            var item = await _unitOfWork.Repository<CommitmentItem>()
                .GetAsync(filter: i => i.Id == itemId && i.CommitmentId == commitmentId && !i.IsDeleted);

            if (item == null)
                throw new InvalidOperationException("Commitment item not found");

            if (!string.IsNullOrWhiteSpace(dto.Description))
                item.UpdateDetails(dto.Description, dto.DetailedDescription, dto.Specifications);

            if (dto.Quantity.HasValue && dto.UnitPrice.HasValue)
                item.UpdateQuantityAndPrice(dto.Quantity.Value, dto.UnitPrice.Value);

            if (dto.DiscountPercentage.HasValue || dto.DiscountAmount.HasValue)
                item.SetDiscount(dto.DiscountPercentage, dto.DiscountAmount);

            if (dto.TaxRate.HasValue)
                item.SetTax(dto.TaxRate);

            item.SetDeliveryInfo(dto.RequiredDate, dto.PromisedDate, dto.DeliveryLocation, null);
            item.SetReferences(dto.DrawingNumber, null, dto.MaterialCode, dto.VendorItemCode);

            var currentUser = _currentUserService.UserId;
            item.UpdatedBy = currentUser;

            _unitOfWork.Repository<CommitmentItem>().Update(item);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitmentId) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<bool> RemoveCommitmentItemAsync(Guid commitmentId, Guid itemId)
        {
            var item = await _unitOfWork.Repository<CommitmentItem>()
                .GetAsync(filter: i => i.Id == itemId && i.CommitmentId == commitmentId && !i.IsDeleted);

            if (item == null)
                return false;

            if (item.InvoicedAmount > 0)
                throw new InvalidOperationException("Cannot remove item with invoiced amounts");

            item.IsDeleted = true;
            item.DeletedAt = DateTime.UtcNow;
            item.DeletedBy = _currentUserService.UserId;

            _unitOfWork.Repository<CommitmentItem>().Update(item);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<CommitmentItemDto>> GetCommitmentItemsAsync(Guid commitmentId)
        {
            var items = await _unitOfWork.Repository<CommitmentItem>()
                .GetAllAsync(
                    filter: i => i.CommitmentId == commitmentId && !i.IsDeleted,
                    includeProperties: "BudgetItem");

            return _mapper.Map<List<CommitmentItemDto>>(items);
        }

        public async Task<CommitmentDto> RecordInvoiceAsync(Guid commitmentId, RecordCommitmentInvoiceDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.RecordInvoice(dto.InvoiceAmount, dto.PaidAmount, dto.RetentionAmount);

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<CommitmentDto> UpdatePerformanceAsync(Guid commitmentId, UpdateCommitmentPerformanceDto dto)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            commitment.UpdatePerformance(dto.PerformancePercentage, dto.ExpectedCompletionDate);

            var currentUser = _currentUserService.UserId;
            commitment.UpdatedBy = currentUser;

            _unitOfWork.Repository<Commitment>().Update(commitment);
            await _unitOfWork.SaveChangesAsync();

            return await GetCommitmentAsync(commitment.Id) ?? throw new InvalidOperationException("Failed to retrieve commitment");
        }

        public async Task<List<CommitmentRevisionDto>> GetCommitmentRevisionsAsync(Guid commitmentId)
        {
            var revisions = await _unitOfWork.Repository<CommitmentRevision>()
                .GetAllAsync(
                    filter: r => r.CommitmentId == commitmentId,
                    orderBy: q => q.OrderBy(r => r.RevisionNumber));

            return _mapper.Map<List<CommitmentRevisionDto>>(revisions);
        }

        public async Task<CommitmentFinancialSummary> GetFinancialSummaryAsync(Guid commitmentId)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(
                    filter: c => c.Id == commitmentId && !c.IsDeleted,
                    includeProperties: "Items,Invoices,Revisions,WorkPackageAllocations");

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            return new CommitmentFinancialSummary
            {
                OriginalBudget = commitment.OriginalAmount,
                CurrentBudget = commitment.RevisedAmount,
                TotalCommitted = commitment.CommittedAmount,
                TotalInvoiced = commitment.InvoicedAmount,
                TotalPaid = commitment.PaidAmount,
                TotalRetention = commitment.RetentionAmount,
                TotalOutstanding = commitment.InvoicedAmount - commitment.PaidAmount,
                BudgetVariance = commitment.RevisedAmount - commitment.OriginalAmount,
                BudgetVariancePercentage = commitment.OriginalAmount > 0 ? 
                    ((commitment.RevisedAmount - commitment.OriginalAmount) / commitment.OriginalAmount) * 100 : 0,
                TotalWorkPackages = commitment.WorkPackageAllocations?.Count ?? 0,
                ActiveWorkPackages = commitment.WorkPackageAllocations?.Count(w => w.AllocatedAmount > 0) ?? 0,
                AverageWorkPackageUtilization = 0 // Would need to calculate based on actual work package data
            };
        }

        public async Task<CommitmentPerformanceMetrics> GetPerformanceMetricsAsync(Guid commitmentId)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(
                    filter: c => c.Id == commitmentId && !c.IsDeleted,
                    includeProperties: "Items,Invoices");

            if (commitment == null)
                throw new InvalidOperationException("Commitment not found");

            return new CommitmentPerformanceMetrics
            {
                StartDate = commitment.StartDate,
                EndDate = commitment.EndDate,
                InvoicingEfficiency = commitment.CommittedAmount > 0 ? 
                    (commitment.InvoicedAmount / commitment.CommittedAmount) * 100 : 0,
                PaymentEfficiency = commitment.InvoicedAmount > 0 ? 
                    (commitment.PaidAmount / commitment.InvoicedAmount) * 100 : 0,
                CostPerformanceIndex = 1.0m, // Would need actual earned value data
                TotalInvoices = commitment.Invoices?.Count ?? 0,
                ApprovedInvoices = commitment.Invoices?.Count(i => i.Status == InvoiceStatus.Approved) ?? 0,
                RejectedInvoices = commitment.Invoices?.Count(i => i.Status == InvoiceStatus.Rejected) ?? 0,
                InvoiceApprovalRate = commitment.Invoices?.Any() == true ?
                    (commitment.Invoices.Count(i => i.Status == InvoiceStatus.Approved) / (decimal)commitment.Invoices.Count) * 100 : 0,
                IsDelayed = DateTime.UtcNow > commitment.EndDate && commitment.Status == CommitmentStatus.Active,
                IsBudgetExceeded = commitment.IsOverCommitted(),
                RiskScore = CalculateRiskScore(commitment)
            };
        }

        public async Task<byte[]> ExportCommitmentsAsync(CommitmentFilterDto filter, string format = "xlsx")
        {
            var commitments = await GetCommitmentsAsync(filter);
            
            // For now, implement a simple CSV export
            var csv = new StringBuilder();
            csv.AppendLine("Commitment Number,Title,Type,Status,Contractor,Original Amount,Revised Amount,Committed Amount,Invoiced Amount,Paid Amount,Currency,Contract Date,Start Date,End Date");
            
            foreach (var commitment in commitments.Items)
            {
                csv.AppendLine($"{commitment.CommitmentNumber},{commitment.Title},{commitment.TypeDescription},{commitment.StatusDescription}," +
                    $"{commitment.ContractorName},{commitment.OriginalAmount},{commitment.RevisedAmount},{commitment.CommittedAmount}," +
                    $"{commitment.InvoicedAmount},{commitment.PaidAmount},{commitment.Currency}," +
                    $"{commitment.ContractDate:yyyy-MM-dd},{commitment.StartDate:yyyy-MM-dd},{commitment.EndDate:yyyy-MM-dd}");
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        public async Task<bool> IsCommitmentNumberUniqueAsync(string commitmentNumber, Guid? excludeId = null)
        {
            var query = _unitOfWork.Repository<Commitment>()
                .Query()
                .Where(c => c.CommitmentNumber == commitmentNumber && !c.IsDeleted);

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> CanDeleteCommitmentAsync(Guid commitmentId)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                return false;

            return commitment.Status == CommitmentStatus.Draft && commitment.InvoicedAmount == 0;
        }

        public async Task<bool> CanReviseCommitmentAsync(Guid commitmentId, decimal newAmount)
        {
            var commitment = await _unitOfWork.Repository<Commitment>()
                .GetAsync(filter: c => c.Id == commitmentId && !c.IsDeleted);

            if (commitment == null)
                return false;

            return (commitment.Status == CommitmentStatus.Active || commitment.Status == CommitmentStatus.PartiallyInvoiced) &&
                   newAmount >= commitment.InvoicedAmount;
        }

        private decimal CalculateRiskScore(Commitment commitment)
        {
            decimal score = 0;

            // Budget risk (30 points)
            if (commitment.IsOverCommitted())
                score += 30;
            else if (commitment.CommittedAmount > commitment.RevisedAmount * 0.9m)
                score += 20;
            else if (commitment.CommittedAmount > commitment.RevisedAmount * 0.8m)
                score += 10;

            // Schedule risk (30 points)
            var daysRemaining = (commitment.EndDate - DateTime.UtcNow).Days;
            var totalDays = (commitment.EndDate - commitment.StartDate).Days;
            var progressPercentage = totalDays > 0 ? (totalDays - daysRemaining) / (decimal)totalDays * 100 : 100;
            var deliveryPercentage = commitment.GetDeliveredPercentage();

            if (daysRemaining < 0) // Overdue
                score += 30;
            else if (progressPercentage > deliveryPercentage + 20) // Behind schedule
                score += 20;
            else if (progressPercentage > deliveryPercentage + 10)
                score += 10;

            // Payment risk (20 points)
            var unpaidAmount = commitment.InvoicedAmount - commitment.PaidAmount;
            if (unpaidAmount > commitment.RevisedAmount * 0.5m)
                score += 20;
            else if (unpaidAmount > commitment.RevisedAmount * 0.3m)
                score += 10;

            // Performance risk (20 points)
            if (commitment.PerformancePercentage.HasValue && commitment.PerformancePercentage < 70)
                score += 20;
            else if (commitment.PerformancePercentage.HasValue && commitment.PerformancePercentage < 80)
                score += 10;

            return Math.Min(score, 100); // Cap at 100
        }
    }
}