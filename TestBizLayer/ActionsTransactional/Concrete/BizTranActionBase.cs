// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsTransactional.Concrete
{
    public abstract class BizTranActionBase : BizActionStatus
    {
        protected Action<BizDataGuid> CallOnSuccess;

        protected BizTranActionBase(int stageNum)
        {
            _stageNum = stageNum;
        }

        public int _stageNum { get; private set; }

        public BizDataGuid BizAction(BizDataGuid input)
        {
            if (input.ShouldError(_stageNum))
            {
                AddError("Error");
            }
            else
            {
                Message = "All Ok";
                CallOnSuccess?.Invoke(input);
            }

            return input;
        }
    }
}