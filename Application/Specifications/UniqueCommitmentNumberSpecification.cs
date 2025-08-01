using Core.DTOs.Cost;
using Domain.Entities.Cost;
using Domain.Interfaces;
using System.Linq.Expressions;

namespace Application.Specifications;

/// <summary>
/// Specification for checking unique commitment number
/// </summary>
public class UniqueCommitmentNumberSpecification : BaseSpecification<Commitment>
{
    public UniqueCommitmentNumberSpecification(string commitmentNumber, Guid? excludeId = null)
    {
        Expression<Func<Commitment, bool>> criteria = c => c.CommitmentNumber == commitmentNumber;

        if (excludeId.HasValue)
        {
            criteria = CombineAndAlso(criteria, c => c.Id != excludeId.Value);
        }

        AddCriteria(criteria);
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