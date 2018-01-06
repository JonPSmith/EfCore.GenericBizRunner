using DataLayer.EfClasses;
using GenericBizRunner;

namespace BizLogic.Orders
{
    public interface IPlaceOrderPart1 : IGenericAction<PlaceOrderInDto, Part1ToPart2Dto> { }
}