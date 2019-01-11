// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BizLogic.Orders;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericBizRunner;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.CheckoutServices;
using ServiceLayer.CheckoutServices.Concrete;
using ExampleWebApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace EfCoreInAction.Controllers
{
    public class CheckoutController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public CheckoutController(EfCoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            SetupTraceInfo();
            return View(listService.GetCheckoutInfoFromCookie());
        }

        public IActionResult Buy(OrderLineItem itemToBuy)
        {
            var cookie = new CheckoutCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var service = new CheckoutCookieService(cookie.GetValue());
            service.AddLineItem(itemToBuy);
            cookie.AddOrUpdateCookie(service.EncodeForCookie());
            SetupTraceInfo();
            return RedirectToAction("Index");
        }

        public IActionResult DeleteLineItem(int lineNum)
        {
            var cookie = new CheckoutCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var service = new CheckoutCookieService(cookie.GetValue());
            service.DeleteLineItem(lineNum);
            cookie.AddOrUpdateCookie(service.EncodeForCookie());
            SetupTraceInfo();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PlaceOrder(PlaceOrderInDto dto, 
            [FromServices]IActionService<IPlaceOrderAction> service)
        {    
            if (!ModelState.IsValid)
            {
                //model errors so return to checkout page, showing the basket
                return View("Index", FormCheckoutDtoFromCookie(HttpContext));
            }

            //This runs my business logic using the service injected into the Action's parameters 
            var orderDto = service.RunBizAction<OrderIdDto>(dto);

            if (!service.Status.HasErrors)
            {
                //If successful I need to clear the line items from the basket cookie
                ClearCheckoutCookie(HttpContext);
                SetupTraceInfo();       //Used to update the logs
                return RedirectToAction("ConfirmOrder", "Orders", 
                    new { orderDto.OrderId, Message = "Your order is confirmed" });
            }

            //Otherwise errors, so I need to redisplay the basket from the cookie
            var checkoutDto = FormCheckoutDtoFromCookie(HttpContext);      
            //This copies the errors to the ModelState
            service.Status.CopyErrorsToModelState(ModelState, checkoutDto);

            SetupTraceInfo();       //Used to update the logs
            return View("Index", checkoutDto);
        }

        //----------------------------------------------------
        //private methods

        private CheckoutDto FormCheckoutDtoFromCookie(HttpContext httpContext)
        {
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            return listService.GetCheckoutInfoFromCookie();
        }

        private void ClearCheckoutCookie(HttpContext httpContext)
        {
            var checkoutCookie = new CheckoutCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var cookieService = new CheckoutCookieService(checkoutCookie.GetValue());
            cookieService.ClearAllLineItems();
            checkoutCookie.AddOrUpdateCookie(cookieService.EncodeForCookie());
        }
    }
}
