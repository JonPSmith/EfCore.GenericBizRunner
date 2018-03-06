using DataLayer.EfClasses;

namespace ServiceLayer.BookServices
{
    public interface IAddReviewService
    {
        AddReviewDto GetOriginal(int id) ;

        Book AddReviewToBook(AddReviewDto dto);
    }
}