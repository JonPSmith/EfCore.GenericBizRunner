// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.ActionsAsync.Concrete
{
    public class BizActionInOnlyWriteDbAsync : BizActionStatus, IBizActionInOnlyWriteDbAsync
    {
        private readonly TestDbContext _context;

        public BizActionInOnlyWriteDbAsync(TestDbContext context)
        {
            _context = context;
        }

        public async Task BizActionAsync(BizDataIn inputData)
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
        }
    }
}
