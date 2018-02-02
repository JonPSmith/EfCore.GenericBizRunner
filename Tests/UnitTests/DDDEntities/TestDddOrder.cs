// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;
using Tests.Helpers;

namespace Tests.UnitTests.DDDEntities
{
    public class TestDddOrder
    {
        [Fact]
        public void TestCreateOrderWithCorrectLineNumsOk()
        {
            //SETUP
            var book1 = DddEfTestData.CreateDummyBookOneAuthor();
            var book2 = DddEfTestData.CreateDummyBookOneAuthor();

            //ATTEMPT
            var lineItems = new List<LineItem> { new LineItem(1, book1), new LineItem(2, book2) };
            var order = new Order("user", DateTime.Today.AddDays(3), lineItems);

            //VERIFY
            order.LineItems.Count.ShouldEqual(2);
            order.LineItems[0].LineNum.ShouldEqual((byte)1);
            order.LineItems[1].LineNum.ShouldEqual((byte)2);
        }

        [Fact]
        public void TestCreateOrderCorrectBookInfoOk()
        {
            //SETUP
            var book = DddEfTestData.CreateDummyBookOneAuthor();

            //ATTEMPT
            var lineItems = new List<LineItem> { new LineItem(3, book) };
            var order = new Order("user", DateTime.Today.AddDays(3), lineItems);

            //VERIFY
            order.LineItems.Count.ShouldEqual(1);
            order.LineItems[0].NumBooks.ShouldEqual((short)3);
            order.LineItems[0].BookPrice.ShouldEqual(book.ActualPrice);
        }

        [Fact]
        public void TestChangeDeliveryDateOk()
        {
            //SETUP
            var book = DddEfTestData.CreateDummyBookOneAuthor();
            var lineItems = new List<LineItem> { new LineItem(3, book) };
            var order = new Order("user", DateTime.Today.AddDays(1), lineItems);

            //ATTEMPT
            var newDeliverDate = DateTime.Today.AddDays(2);
            if (newDeliverDate.DayOfWeek == DayOfWeek.Sunday)
                newDeliverDate = newDeliverDate.AddDays(1);
            var status = order.ChangeDeliveryDate("user", newDeliverDate);

            //VERIFY
            status.HasErrors.ShouldBeFalse();
            order.ExpectedDeliveryDate.ShouldEqual(newDeliverDate);
        }

        [Fact]
        public void TestCreateOrderAndAddToDbOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books.First();
                var lineItems = new List<LineItem> {new LineItem(1, book)};
                context.Add( new Order("user", DateTime.Today.AddDays(3), lineItems));
                context.SaveChanges();

                //VERIFY
                context.Orders.Count().ShouldEqual(1);
                context.Set<LineItem>().Count().ShouldEqual(1);
            }
        }

        

    }

}