using Core.DTOs.Cost.Commitments;
using Core.Enums.Cost;
using Domain.Entities.Cost.Commitments;
namespace Application.Specifications;

/// <summary>
/// Specification for filtering and querying commitments
/// </summary>
public class CommitmentFilterSpecification : BaseSpecification<Commitment>
{
    public CommitmentFilterSpecification(CommitmentFilterDto filter)
    {
        // Build criteria expression
        Expression<Func<Commitment, bool>> criteria = c => !c.IsDeleted;

        // Apply project filter
        if (filter.ProjectId.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.ProjectId == filter.ProjectId.Value);
        }

        // Apply contractor filter
        if (filter.ContractorId.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.ContractorId == filter.ContractorId.Value);
        }

        // Apply control account filter
        if (filter.ControlAccountId.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.ControlAccountId == filter.ControlAccountId.Value);
        }

        // Apply budget item filter
        if (filter.BudgetItemId.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.BudgetItemId == filter.BudgetItemId.Value);
        }

        // Apply type filter
        if (filter.Type.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.Type == filter.Type.Value);
        }

        // Apply status filter
        if (filter.Status.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.Status == filter.Status.Value);
        }

        // Apply text search
        if (!string.IsNullOrWhiteSpace(filter.SearchText))
        {
            var searchTerm = filter.SearchText.ToLower();
            Expression<Func<Commitment, bool>> searchCriteria = c =>
                c.CommitmentNumber.ToLower().Contains(searchTerm) ||
                c.Title.ToLower().Contains(searchTerm) ||
                (c.Description != null && c.Description.ToLower().Contains(searchTerm)) ||
                (c.ContractNumber != null && c.ContractNumber.ToLower().Contains(searchTerm)) ||
                (c.PurchaseOrderNumber != null && c.PurchaseOrderNumber.ToLower().Contains(searchTerm)) ||
                (c.VendorReference != null && c.VendorReference.ToLower().Contains(searchTerm));

            criteria = CombineAndAlso(criteria, searchCriteria);
        }

        // Apply specific number searches
        if (!string.IsNullOrWhiteSpace(filter.CommitmentNumber))
        {
            var number = filter.CommitmentNumber.ToLower();
            criteria = CombineAndAlso(criteria, c => c.CommitmentNumber.ToLower().Contains(number));
        }

        if (!string.IsNullOrWhiteSpace(filter.ContractNumber))
        {
            var number = filter.ContractNumber.ToLower();
            criteria = CombineAndAlso(criteria, c => c.ContractNumber != null &&
                c.ContractNumber.ToLower().Contains(number));
        }

        if (!string.IsNullOrWhiteSpace(filter.PurchaseOrderNumber))
        {
            var number = filter.PurchaseOrderNumber.ToLower();
            criteria = CombineAndAlso(criteria, c => c.PurchaseOrderNumber != null &&
                c.PurchaseOrderNumber.ToLower().Contains(number));
        }

        // Apply date range filters
        if (filter.DateFrom.HasValue)
        {
            var dateFrom = filter.DateFrom.Value.Date;
            criteria = CombineAndAlso(criteria, c => c.ContractDate >= dateFrom);
        }

        if (filter.DateTo.HasValue)
        {
            var dateTo = filter.DateTo.Value.Date;
            criteria = CombineAndAlso(criteria, c => c.ContractDate <= dateTo);
        }

        // Apply amount range filters
        if (filter.AmountMin.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.CommittedAmount >= filter.AmountMin.Value);
        }

        if (filter.AmountMax.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.CommittedAmount <= filter.AmountMax.Value);
        }

        // Apply status flags
        if (filter.IsActive.HasValue)
        {
            if (filter.IsActive.Value)
            {
                criteria = CombineAndAlso(criteria, c =>
                    c.Status == CommitmentStatus.Active ||
                    c.Status == CommitmentStatus.PartiallyInvoiced);
            }
            else
            {
                criteria = CombineAndAlso(criteria, c =>
                    c.Status == CommitmentStatus.Closed ||
                    c.Status == CommitmentStatus.Cancelled);
            }
        }

        if (filter.IsOverCommitted.HasValue && filter.IsOverCommitted.Value)
        {
            criteria = CombineAndAlso(criteria, c => c.InvoicedAmount > c.CommittedAmount);
        }

        if (filter.IsExpired.HasValue && filter.IsExpired.Value)
        {
            var now = DateTime.UtcNow;
            criteria = CombineAndAlso(criteria, c =>
                now > c.EndDate && c.Status == CommitmentStatus.Active);
        }

        // Set the final criteria
        AddCriteria(criteria);

        // Add includes
        AddInclude(c => c.Contractor!);
        AddInclude(c => c.BudgetItem!);
        AddInclude(c => c.ControlAccount!);
        AddInclude(c => c.Project);

        // Apply sorting
        ApplySorting(filter);

        // Apply pagination
        if (filter.PageNumber > 0 && filter.PageSize > 0)
        {
            ApplyPaging((filter.PageNumber - 1) * filter.PageSize, filter.PageSize);
        }
    }

    private void ApplySorting(CommitmentFilterDto filter)
    {
        var sortBy = filter.SortBy?.ToLower() ?? "commitmentnumber";

        switch (sortBy)
        {
            case "commitmentnumber":
                if (filter.SortDescending)
                    ApplyOrderByDescending(c => c.CommitmentNumber);
                else
                    ApplyOrderBy(c => c.CommitmentNumber);
                break;

            case "title":
                if (filter.SortDescending)
                    ApplyOrderByDescending(c => c.Title);
                else
                    ApplyOrderBy(c => c.Title);
                break;

            case "contractdate":
                if (filter.SortDescending)
                    ApplyOrderByDescending(c => c.ContractDate);
                else
                    ApplyOrderBy(c => c.ContractDate);
                break;

            case "committedamount":
                if (filter.SortDescending)
                    ApplyOrderByDescending(c => c.CommittedAmount);
                else
                    ApplyOrderBy(c => c.CommittedAmount);
                break;

            case "status":
                if (filter.SortDescending)
                    ApplyOrderByDescending(c => (int)c.Status);
                else
                    ApplyOrderBy(c => (int)c.Status);
                break;

            case "contractor":
                if (filter.SortDescending)
                    ApplyOrderByDescending(c => c.Contractor!.Name);
                else
                    ApplyOrderBy(c => c.Contractor!.Name);
                break;

            case "createdat":
                if (filter.SortDescending)
                    ApplyOrderByDescending(c => c.CreatedAt);
                else
                    ApplyOrderBy(c => c.CreatedAt);
                break;

            default:
                ApplyOrderBy(c => c.CommitmentNumber);
                break;
        }
    }

    private Expression<Func<T, bool>> CombineAndAlso<T>(
        Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left, right), parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression? node)
        {
            if (node == _oldValue)
                return _newValue;
            return base.Visit(node)!;
        }
    }
}
