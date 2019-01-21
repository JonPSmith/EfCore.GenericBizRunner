// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;
using TestBizLayer.DbForTransactions;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionInOnlyWriteDbForceValidateOff : BizActionStatus, IBizActionInOnlyWriteDbForceValidateOff
    {
        private readonly TestDbContext _context;

        public BizActionInOnlyWriteDbForceValidateOff(TestDbContext context)
        {
            _context = context;
        }

        protected override ValidateOnSaveStates ValidateOnSaveSetting => ValidateOnSaveStates.DoNotValidate;

        public void BizAction(BizDataIn inputData)
        {
            //Put it here so that if SaveChanges is called it will be in database
            _context.Add(new LogEntry(inputData.Num.ToString()));
            if (inputData.Num >= 0)
            {
                Message = "All Ok";
            }
            else
            {
                AddError("Error");
            }
        }
    }
}
