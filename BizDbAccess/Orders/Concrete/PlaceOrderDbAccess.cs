// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;

namespace BizDbAccess.Orders.Concrete
{
    public class PlaceOrderDbAccess : IPlaceOrderDbAccess
    {
        private readonly EfCoreContext _context;

        public PlaceOrderDbAccess(EfCoreContext context)//#A
        {
            _context = context;
        }

        public Book FindBook(int bookId)
        {
            return _context.Find<Book>(bookId);
        }

        public void Add(Order newOrder)                 //#G
        {                                               //#G
            _context.Orders.Add(newOrder);              //#G
        }                                               //#G
    }
    /************************************************************
    #A All the BizDbAccess need the application's DbContext to access the database
    #B This method finds all the books that the user wants to buy
    #C The BizLogic hands it a collection of BookIds, which the checkout has provided
    #D This finds a book, if present, for each Id. 
    #E I include any optional promotion, as the BizLogic needs that for working out the price
    #F I return the result as a dictionary to make it easier for the BizLogic to look them up
    #G This method simply adds the new order that the BizlOgic built into the DbContext's Orders DbSet collection
     * **********************************************************/
}