using DataLayer.EfClasses;
using GenericBizRunner;

namespace BizLogic.Orders
{
    public interface IPlaceOrderAction : IGenericActionWriteDb<PlaceOrderInDto, Order> { }
}