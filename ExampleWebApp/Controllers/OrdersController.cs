// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using BizLogic.Orders;
using DataLayer.EfCode;
using GenericBizRunner;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.CheckoutServices.Concrete;
using ServiceLayer.OrderServices;
using ServiceLayer.OrderServices.Concrete;

namespace EfCoreInAction.Controllers
{
    public class OrdersController : BaseTraceController
    {
        private readonly EfCoreContext _context;

        public OrdersController(EfCoreContext context)
        {
            _context = context;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var listService = new DisplayOrdersService(_context);
            SetupTraceInfo();
            return View(listService.GetUsersOrders(HttpContext.Request.Cookies));
        }

        public IActionResult ConfirmOrder(int orderId)
        {
            var detailService = new DisplayOrdersService(_context);
            SetupTraceInfo();
            return View(detailService.GetOrderDetail(orderId));
        }

        public IActionResult ChangeDelivery(int id, [FromServices]IActionService<OrderDbContext, IChangeDeliverAction> service)
        {
            //When we start the DTO doesn't have the orderId or UserId. We need to add them here
            var dto = service.GetDto<GenericBizChangeDeliveryDto>(x =>
            {
                x.OrderId = id;
                x.UserId = GetUserId(HttpContext);
            });
            return View(dto);
        }

        //---------------------------------------------------------------
        //private methods

        private string GetUserId(HttpContext httpContext)
        {
            var checkoutCookie = new CheckoutCookie(HttpContext.Request.Cookies, HttpContext.Response.Cookies);
            var cookieService = new CheckoutCookieService(checkoutCookie.GetValue());
            return cookieService.UserId;
        }
    }
}
