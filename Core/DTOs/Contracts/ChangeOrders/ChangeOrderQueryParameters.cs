using Core.DTOs.Common;
using Core.Enums.Change;
using Core.Enums.Cost;

namespace Core.DTOs.Contracts.ChangeOrders;

public class ChangeOrderQueryParameters : SimpleQueryParameters
{
    public Guid? ContractId { get; set; }
    public ChangeOrderStatus? Status { get; set; }
    public ChangeOrderType? Type { get; set; }
    public ChangeOrderPriority? Priority { get; set; }
}