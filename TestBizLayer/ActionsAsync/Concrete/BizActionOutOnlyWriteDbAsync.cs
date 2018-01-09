// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.ActionsAsync.Concrete
{
    public class BizActionOutOnlyWriteDbAsync : BizActionStatus, IBizActionOutOnlyWriteDbAsync
    {
        private readonly TestDbContext _context;

        public BizActionOutOnlyWriteDbAsync(TestDbContext context)
        {
            _context = context;
        }

        public async Task<BizDataOut> BizActionAsync()
        {
            _context.Add(new LogEntry(GetType().Name));
            Message = "All Ok";
            return new BizDataOut("Result");
        }
    }
}
