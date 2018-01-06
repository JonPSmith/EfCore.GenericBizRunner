using DataLayer.EfClasses;
using GenericBizRunner;

namespace BizLogic.Orders
{
    public interface IPlaceOrderAction : IGenericAction<PlaceOrderInDto, Order> { }
}