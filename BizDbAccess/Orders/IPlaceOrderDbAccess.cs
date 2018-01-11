using System.Collections.Generic;
using DataLayer.EfClasses;

namespace BizDbAccess.Orders
{
    public interface IPlaceOrderDbAccess
    {
        /// <summary>
        /// This finds any books that fits the BookIds given to it
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns>A dictionary with the BookId as the key, and the Book as the value</returns>
        IDictionary<int, Book> FindBooksByIdsWithPriceOffers(IEnumerable<int> bookIds);

        void Add(Order newOrder);
    }
}