using DataLayer.EfClasses;

namespace ServiceLayer.BookServices
{
    public interface IChangePubDateService
    {
        ChangePubDateDto GetOriginal(int id);
        Book UpdateBook(ChangePubDateDto dto);
    }
}