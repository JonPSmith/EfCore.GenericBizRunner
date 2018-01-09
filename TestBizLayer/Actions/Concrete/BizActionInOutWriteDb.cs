// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionInOutWriteDb : BizActionStatus, IBizActionInOutWriteDb
    {
        private readonly TestDbContext _context;

        public BizActionInOutWriteDb(TestDbContext context)
        {
            _context = context;
        }

        public BizDataOut BizAction(BizDataIn inputData)
        {
            if (inputData.Num >= 0)
            {
                _context.Add(new LogEntry(inputData.Num.ToString()));
                Message = "All Ok";
            }
            else
            {
                AddError("Error");
            }
            return new BizDataOut("Result");
        }
    }
}
