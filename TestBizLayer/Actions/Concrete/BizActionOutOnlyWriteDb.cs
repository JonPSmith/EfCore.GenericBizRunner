// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionOutOnlyWriteDb : BizActionStatus, IBizActionOutOnlyWriteDb
    {
        private readonly TestDbContext _context;

        public BizActionOutOnlyWriteDb(TestDbContext context)
        {
            _context = context;
        }

        public BizDataOut BizAction()
        {
            Message = "All Ok";
            _context.Add(new LogEntry(GetType().Name));
            return new BizDataOut("Result");
        }
    }
}
