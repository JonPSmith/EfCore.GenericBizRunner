// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using DataLayer.EfCode;
using EfCoreInAction.DatabaseHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace ServiceLayer.DatabaseServices.Concrete
{
    public enum DbStartupModes { UseExisting, EnsureCreated, EnsureDeletedCreated, UseMigrations}

    public static class SetupHelpers
    {

        private const string SeedDataSearchName = "Apress books*.json";
        public const string SeedFileSubDirectory = "seedData";
        private const decimal DefaultBookPrice = 40;    //Any book without a price is set to this value

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
                books.Where(x => x.Price == -1).ToList().ForEach(x => x.Price = DefaultBookPrice);
                context.Books.AddRange(books);
                context.SaveChanges();
                context.SaveChanges();
                numBooks = books.Count + 1;
            }

            return numBooks;
        }
    }
}