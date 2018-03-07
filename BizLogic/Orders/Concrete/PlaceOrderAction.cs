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

        public PlaceOrderAction(IPlaceOrderDbAccess dbAccess)
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

            var bookOrders = 
                dto.LineItems.Select(  
                    x => _dbAccess.BuildBooksDto(x.BookId, x.NumBooks)); 
            var orderStatus = Order.CreateOrderFactory(
                dto.UserId, DateTime.Today.AddDays(5),
                bookOrders);
            CombineErrors(orderStatus);

            if (!HasErrors)
                _dbAccess.Add(orderStatus.Result);

            return HasErrors ? null : orderStatus.Result;
        }
    }
}