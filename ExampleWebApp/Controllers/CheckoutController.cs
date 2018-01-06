// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using BizLogic.Orders;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using GenericBizRunner;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.CheckoutServices.Concrete;
using ServiceLayer.OrderServices.Concrete;

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
            return View(listService.GetCheckoutList());
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

        public IActionResult PlaceOrder(bool acceptTAndCs, [FromServices]IActionService<IPlaceOrderAction> service)
        {
            var checkoutCookie = new CheckoutCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var checkoutService = new CheckoutCookieService(checkoutCookie.GetValue());

            var inputData = new PlaceOrderInDto(acceptTAndCs, checkoutService.UserId, checkoutService.LineItems);
            var order = service.RunBizAction<Order>(inputData);
            var orderId = order.OrderId;

            if (!service.Status.HasErrors)
                return RedirectToAction("ConfirmOrder", "Orders", new {orderId});

            //Otherwise errors, so copy over and redisplay
            foreach (var error in service.Status.Errors)
            {
                var properties = error.MemberNames.ToList();
                ModelState.AddModelError(properties.Any() ? properties.First() : "", error.ErrorMessage);
            }
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            SetupTraceInfo();
            return View(listService.GetCheckoutList());
        }
    }
}
