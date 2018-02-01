// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BizDbAccess.Orders;
using DataLayer.EfClasses;
using GenericBizRunner;

namespace BizLogic.Orders.Concrete
{
    public class PlaceOrderAction : BizActionStatus, IPlaceOrderAction
    {
        private readonly IPlaceOrderDbAccess _dbAccess;

        public PlaceOrderAction(IPlaceOrderDbAccess dbAccess)//#C
        {
            _dbAccess = dbAccess;
        }

        /// <summary>
        /// This validates the input and if OK creates 
        /// an order and calls the _dbAccess to add to orders
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>returns an Order. Will be null if there are errors</returns>
        public Order BizAction(PlaceOrderInDto dto) 
        {
            if (!dto.AcceptTAndCs)                    
            {                                         
                AddError("You must accept the T&Cs to place an order.");   
                return null;                          
            }                                         
            if (!dto.LineItems.Any())                 
            {                                         
                AddError("No items in your basket."); 
                return null;                          
            }                                         

            var booksDict = _dbAccess.FindBooksByIdsWithPriceOffers    
                     (dto.LineItems.Select(x => x.BookId));
            var order = new Order(dto.UserId, DateTime.Today.AddDays(5),
                FormLineItemsWithErrorChecking(dto.LineItems, booksDict));                                           

            if (!HasErrors)
                _dbAccess.Add(order);

            return HasErrors ? null : order;
        }

        private List<LineItem>  FormLineItemsWithErrorChecking
            (IEnumerable<OrderLineItem> lineItems,            
             IDictionary<int,Book> booksDict)                 
        {
            var result = new List<LineItem>();
            var i = 1;
            
            foreach (var lineItem in lineItems)
            {
                if (!booksDict.                             
                    ContainsKey(lineItem.BookId))           
                        throw new InvalidOperationException 
                        ($"An order failed because book, id = {lineItem.BookId} was missing.");               

                var book = booksDict[lineItem.BookId];
                var bookPrice = book.ActualPrice; 
                if (bookPrice <= 0)                         
                    AddError($"Sorry, the book '{book.Title}' is not for sale.");    
                else
                {
                    //Valid, so add to the order
                    result.Add(new LineItem((byte) (i++), lineItem.NumBooks, bookPrice, book));
                }
            }
            return result;
        }
    }
}