// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using BizLogic.Orders;
using DataLayer.EfCode;
using ExampleWebApp.Helpers;
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

        public IActionResult ConfirmOrder(int orderId, string message)
        {
            var detailService = new DisplayOrdersService(_context);
            SetupTraceInfo();
            ViewData["Message"] = message ?? "Your order has been updated.";
            return View(detailService.GetOrderDetail(orderId));
        }

        public IActionResult ChangeDelivery(int id, 
            [FromServices]IActionService<IChangeDeliverAction> service)
        {
            //When the DTO is created it will run the SetupSecondaryData method to build the data needed for the display
            //For the SetupSecondaryData to work it needs the orderId and UserId, so the GetDto method allow you to add these
            //via the optional parameter. This sets these properties before the SetupSecondaryData method is called
            var dto = service.GetDto<WebChangeDeliveryDto>(x =>
            {
                x.OrderId = id;
                x.UserId = GetUserId(HttpContext);
            });
            //It is possible that the call to SetupSecondaryData may add an error
            //CopyErrorsToModelState will do notthing if there are no errors
            service.Status.CopyErrorsToModelState(ModelState, dto); 
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeDelivery(WebChangeDeliveryDto dto,
            [FromServices]IActionService<IChangeDeliverAction> service)
        {
            if (!ModelState.IsValid)
            {
                //I have to reset the DTO, which will call SetupSecondaryData
                //to set up the values needed for the display
                service.ResetDto(dto);
                //model errors so return to checkout page, showing the basket
                return View(dto);
            }

            //This runs my business logic using the service injected in the
            //Action's service parameter
            service.RunBizAction(dto);

            if (!service.Status.HasErrors)
            {
                SetupTraceInfo();       //Used to update the logs
                //We copy the message from the business logic to show 
                return RedirectToAction("ConfirmOrder", "Orders", 
                    new { dto.OrderId, message = service.Status.Message });
            }

            //Otherwise errors, so I need to redisplay the page to the user
            service.Status.CopyErrorsToModelState(ModelState, dto);
            //I reset the DTO, which will call SetupSecondaryData i set up the display props
            service.ResetDto(dto);
            SetupTraceInfo();       //Used to update the logs
            return View(dto); //redisplay the page, with the errors
        }

        //---------------------------------------------------------------
        //private methods

        private string GetUserId(HttpContext httpContext)
        {
            var checkoutCookie = new CheckoutCookie(httpContext.Request.Cookies, httpContext.Response.Cookies);
            var cookieService = new CheckoutCookieService(checkoutCookie.GetValue());
            return cookieService.UserId;
        }
    }
}
