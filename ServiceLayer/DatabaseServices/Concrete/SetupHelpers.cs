// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using ServiceLayer.CheckoutServices.Concrete;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public enum DbStartupModes { UseExisting, EnsureCreated, EnsureDeletedCreated, UseMigrations}

    public static class SetupHelpers
    {

        private const string SeedDataSearchName = "Apress books*.json";
        public const string SeedFileSubDirectory = "seedData";
        private const decimal DefaultBookPrice = 40;    //Any book without a price is set to this value

        public static void DevelopmentEnsureDeleted(this EfCoreContext db)
        {
            db.Database.EnsureDeleted();
        }

        public static void DevelopmentEnsureCreated(this EfCoreContext db)
        {
            db.Database.EnsureCreated();
        }

        public static int SeedDatabase(this EfCoreContext context, string dataDirectory)
        {
            if (!(context.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
                throw new InvalidOperationException("The database does not exist. If you are using Migrations then run PMC command update-database to create it");

            var numBooks = context.Books.Count();
            if (numBooks == 0)
            {
                //the database is emply so we fill it from a json file
                var books = BookJsonLoader.LoadBooks(Path.Combine(dataDirectory, SeedFileSubDirectory),
                    SeedDataSearchName).ToList();
                context.Books.AddRange(books);
                context.SaveChanges();
                numBooks = books.Count + 1;

                context.ResetOrders(books);
            }

            return numBooks;
        }

        /// <summary>
        /// This wipes all the existing orders and creates a new set of orders
        /// </summary>
        /// <param name="context"></param>
        /// <param name="books"></param>
        public static void ResetOrders(this EfCoreContext context, List<Book> books = null)
        {
            context.RemoveRange(context.Orders.ToList());        //remove the existing orders (the lineitems will go too)
            context.AddDummyOrders(books);              //add a dummy set of orders
            context.SaveChanges();
        }

        //------------------------------------------------------
        //private methods


        private static readonly string[] DummyUsersIds = new[]
        {
            CheckoutCookieService.DefaultUserId,
            "albert@einstein.com",
            "ada@lovelace.co.uk"
        };
    
        private static void AddDummyOrders(this EfCoreContext context, List<Book> books = null)
        {
            if (books == null)
                books = context.Books.ToList();

            var orders = new List<Order>();
            var i = 0;
            foreach (var usersId in DummyUsersIds)
            {
                orders.Add(BuildOrder(usersId, DateTime.UtcNow.AddDays(-10), books[i++]));
                orders.Add(BuildOrder(usersId, DateTime.UtcNow, books[i++]));
            }
            context.AddRange(orders);
        }

        private static Order BuildOrder(string userId, DateTime orderDate, Book bookOrdered)
        {
            var deliverDay = orderDate.AddDays(5);
            var bookPrice = bookOrdered.ActualPrice;
            return new Order
            {
                CustomerName = userId,
                DateOrderedUtc = orderDate,
                ExpectedDeliveryDate = deliverDay,
                HasBeenDelivered = deliverDay < DateTime.Today,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookPrice = bookPrice,
                        ChosenBook = bookOrdered,
                        NumBooks = 1
                    }
                }
            };
        }
    }
}