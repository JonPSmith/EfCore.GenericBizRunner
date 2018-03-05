using System.Collections.Generic;
using DataLayer.Dtos;
using DataLayer.EfClasses;

namespace BizDbAccess.Orders
{
    public interface IPlaceOrderDbAccess
    {
        OrderBooksDto BuildBooksDto(int bookId, byte numBooks);

        void Add(Order newOrder);
    }
}