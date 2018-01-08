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
        public IActionResult PlaceOrder(PlaceOrderInDto dto, [FromServices]IActionService<IPlaceOrderAction> service)
        {
            var listService = new CheckoutListService(_context, HttpContext.Request.Cookies);
            if (!ModelState.IsValid)
            {
                //model errors so return immediately
                return View("Index", listService.GetCheckoutInfoFromCookie());
            }

            var order = service.RunBizAction<Order>(dto);

            if (!service.Status.HasErrors)
            {
                //todo: clear checkout cookie
                return RedirectToAction("ConfirmOrder", "Orders", new { order.OrderId });
            }

            //Otherwise errors, so copy over and redisplay
            foreach (var error in service.Status.Errors)
            {
                var properties = error.MemberNames.ToList();
                ModelState.AddModelError(properties.Any() ? properties.First() : "", error.ErrorMessage);
            }
            SetupTraceInfo();
            return View("Index", listService.GetCheckoutInfoFromCookie());
        }
    }
}
