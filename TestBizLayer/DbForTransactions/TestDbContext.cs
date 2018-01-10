// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace TestBizLayer.DbForTransactions
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(
            DbContextOptions<TestDbContext> options)
            : base(options) { }

        public DbSet<LogEntry> LogEntries { get; set; }
    }
}