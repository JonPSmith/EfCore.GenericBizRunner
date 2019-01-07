// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionInOut : 
        BizActionStatus, 
        IBizActionInOut
    {
        public BizDataOut BizAction(BizDataIn inputData)
        {
            if (inputData.Num >= 0)
            {
                Message = "All Ok";              
            }
            else
            {
                AddError("Error");
            }
            return new BizDataOut(inputData.Num.ToString());
        }
    }
}
