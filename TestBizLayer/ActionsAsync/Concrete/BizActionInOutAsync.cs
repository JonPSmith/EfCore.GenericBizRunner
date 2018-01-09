// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.


using System.Threading.Tasks;
using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsAsync.Concrete
{
    public class BizActionInOutAsync : BizActionStatus, IBizActionInOutAsync
    {
        public async Task<BizDataOut> BizActionAsync(BizDataIn inputData)
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
