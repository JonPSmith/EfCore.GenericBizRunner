using DataLayer.EfClasses;
using GenericBizRunner;

namespace BizLogic.Orders
{
    public interface IChangeDeliverAction : IGenericActionInOnlyWriteDb<BizChangeDeliverDto> { }
}