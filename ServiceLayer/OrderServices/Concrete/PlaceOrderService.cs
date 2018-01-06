// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using BizDbAccess.Orders;
using BizLogic.Orders;
using BizLogic.Orders.Concrete;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Http;
using ServiceLayer.CheckoutServices.Concrete;

namespace ServiceLayer.OrderServices.Concrete
{
    public class PlaceOrderService
    {
        private readonly CheckoutCookie _checkoutCookie;  //#A
        private readonly 
            RunnerWriteDb<PlaceOrderInDto, Order> _runner;//#B

        public IImmutableList<ValidationResult> 
            Errors => _runner.Errors; //#C

        public PlaceOrderService(              //#D
            IRequestCookieCollection cookiesIn,//#D 
            IResponseCookies cookiesOut,       //#D
            EfCoreContext context)             //#D
        {
            _checkoutCookie = new CheckoutCookie(//#E
                cookiesIn, cookiesOut);          //#E
            _runner = 
                new RunnerWriteDb<PlaceOrderInDto, Order>(
                    new PlaceOrderAction(                 
                        new PlaceOrderDbAccess(context)), 
                    context);                             
        }

        /// <summary>
        /// This creates the order and, if successful clears the cookie
        /// </summary>
        /// <returns>Returns the OrderId, or zero if errors</returns>
        public int PlaceOrder(bool acceptTAndCs)
        {
            var checkoutService = new CheckoutCookieService(
                _checkoutCookie.GetValue());                

            var order = _runner.RunAction(       
                new PlaceOrderInDto(acceptTAndCs,
                checkoutService.UserId,          
                checkoutService.LineItems));     

            if (_runner.HasErrors) return 0; 

            //successful so clear the cookie line items
            checkoutService.ClearAllLineItems();   
            _checkoutCookie.AddOrUpdateCookie(     
                checkoutService.EncodeForCookie());

            return order.OrderId;//#L
        }
    }

}