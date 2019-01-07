// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.ActionsTransactional.Concrete
{
    public class BizTranActionFirstWriteDb : BizTranActionBase, IBizTranActionFirstWriteDb
    {
        private readonly DbContext _db;

        public BizTranActionFirstWriteDb(DbContext db)
            : base(1)
        {
            _db = db;
            CallOnSuccess = SuccessAction;
        }

        private void SuccessAction(BizDataGuid input)
        {

            if (_db != null)
                _db.Set<LogEntry>()
                    .Add(new LogEntry(input.Modes.HasFlag(ActionModes.FirstWriteBad)
                        ? "x"
                        : string.Format("Stage {0}: Guid = {1}", _stageNum, input.Unique)));
        }
    }
}
