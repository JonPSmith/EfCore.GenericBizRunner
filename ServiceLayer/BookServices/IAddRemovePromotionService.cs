using DataLayer.EfClasses;
using GenericBizRunner;

namespace ServiceLayer.BookServices
{
    public interface IAddRemovePromotionService
    {
        IGenericStatus Status { get; }

        AddRemovePromotionDto GetOriginal(int id);

        Book AddPromotion(AddRemovePromotionDto dto);
        Book RemovePromotion(int id);
    }
}