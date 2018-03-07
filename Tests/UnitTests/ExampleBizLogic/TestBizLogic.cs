// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BizDbAccess.Orders.Concrete;
using BizLogic.Orders;
using BizLogic.Orders.Concrete;
using DataLayer.EfCode;
using Tests.Helpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Tests.UnitTests.ExampleBizLogic
{
    public class TestBizLogic
    {
        [Fact]
        public void TestAddOrderViaBizLogicOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new PlaceOrderAction(new PlaceOrderDbAccess(context));
                var dto = new PlaceOrderInDto
                {
                    AcceptTAndCs = true,
                    LineItems = new List<OrderLineItem>
                    {
                        new OrderLineItem {BookId = 1, NumBooks = 2}
                    }
                };

                //ATTEMPT
                var order = service.BizAction(dto);

                //VERIFY
                service.HasErrors.ShouldBeFalse();
                order.LineItems.Count().ShouldEqual(1);
                order.LineItems.Single().BookPrice.ShouldEqual(context.Books.Find(1).ActualPrice);
            }
        }

        [Fact]
        public void TestAddOrderNoAcceptBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var service = new PlaceOrderAction(new PlaceOrderDbAccess(context));
                var dto = new PlaceOrderInDto
                {
                    AcceptTAndCs = false,

                };

                //ATTEMPT
                var order = service.BizAction(dto);

                //VERIFY
                service.HasErrors.ShouldBeTrue();
                service.Errors.Single().ErrorMessage.ShouldEqual("You must accept the T&Cs to place an order.");
            }
        }

        [Fact]
        public void TestChangeDeliverDateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                context.SeedDummyOrder();

                var service = new ChangeDeliveryAction(new ChangeDeliverDbAccess(context));
                var newDeliverDate = DateTime.Today.AddDays(2);
                if (newDeliverDate.DayOfWeek == DayOfWeek.Sunday)
                    newDeliverDate = newDeliverDate.AddDays(1);
                var dto = new BizChangeDeliverDto
                {
                    UserId = DddEfTestData.DummyUserId,
                    OrderId = context.Orders.Single().OrderId,
                    NewDeliveryDate = newDeliverDate
                };

                //ATTEMPT
                service.BizAction(dto);

                //VERIFY
                service.HasErrors.ShouldBeFalse();
                context.Orders.Single().ExpectedDeliveryDate.ShouldEqual(newDeliverDate);
            }
        }



    }

}