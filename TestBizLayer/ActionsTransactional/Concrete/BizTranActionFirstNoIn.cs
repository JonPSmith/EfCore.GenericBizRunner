// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsTransactional.Concrete
{
    public class BizTranActionFirstNoIn : BizActionStatus, IBizTranActionFirstNoIn
    {
        public BizDataGuid BizAction()
        {
            Message = "All Ok";
            return new BizDataGuid(ActionModes.AllOk);
        }
    }
}
