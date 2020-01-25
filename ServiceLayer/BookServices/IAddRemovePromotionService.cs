using DataLayer.EfClasses;
using GenericBizRunner;
using StatusGeneric;

namespace ServiceLayer.BookServices
{
    public interface IAddRemovePromotionService
    {
        IStatusGeneric Status { get; }

        AddRemovePromotionDto GetOriginal(int id);

        Book AddPromotion(AddRemovePromotionDto dto);
        Book RemovePromotion(int id);
    }
}