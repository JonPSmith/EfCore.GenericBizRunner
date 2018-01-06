using DataLayer.EfClasses;
using GenericBizRunner;

namespace BizLogic.Orders
{
    public interface IPlaceOrderPart2 : IGenericAction<Part1ToPart2Dto, Order> { }
}