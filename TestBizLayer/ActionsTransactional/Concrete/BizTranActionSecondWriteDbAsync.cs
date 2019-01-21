// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.ActionsTransactional.Concrete
{
    public class BizTranActionSecondWriteDbAsync : BizTranActionBase, IBizTranActionSecondWriteDbAsync
    {
        private readonly DbContext _db;


        public BizTranActionSecondWriteDbAsync(DbContext db)
            : base(2)
        {
            _db = db;
            CallOnSuccess = SuccessAction;
        }

        public async Task<BizDataGuid> BizActionAsync(BizDataGuid inputData)
        {
            return BizAction(inputData);
        }

        private void SuccessAction(BizDataGuid input)
        {
            if (_db != null)
                _db.Set<LogEntry>()
                    .Add(new LogEntry(input.Modes.HasFlag(ActionModes.SecondWriteBad)
                        ? "x"
                        : string.Format("Stage {0}: Guid = {1}", _stageNum, input.Unique)));
        }
    }
}
