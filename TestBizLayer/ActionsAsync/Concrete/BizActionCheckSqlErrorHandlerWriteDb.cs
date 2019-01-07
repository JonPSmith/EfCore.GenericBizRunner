// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner;
using TestBizLayer.Actions;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.ActionsAsync.Concrete
{
    public class BizActionCheckSqlErrorHandlerWriteDbAsync : BizActionStatus, IBizActionCheckSqlErrorHandlerWriteDbAsync
    {
        private readonly TestDbContext _context;

        public BizActionCheckSqlErrorHandlerWriteDbAsync(TestDbContext context)
        {
            _context = context;
        }

        public async Task BizActionAsync(string setUnique)
        {
            _context.Add(new UniqueEntity{ UniqueString = setUnique});
        }
    }
}
