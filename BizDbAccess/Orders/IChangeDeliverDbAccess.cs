using DataLayer.EfClasses;

namespace BizDbAccess.Orders
{
    public interface IChangeDeliverDbAccess
    {
        Order GetOrder(int orderId);
    }
}