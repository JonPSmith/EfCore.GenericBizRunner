using System.Collections.Generic;
using DataLayer.EfClasses;

namespace BizDbAccess.Orders
{
    public interface IPlaceOrderDbAccess
    {
        Book FindBook(int bookId);

        void Add(Order newOrder);
    }
}