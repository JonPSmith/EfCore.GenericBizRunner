// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsAsync.Concrete
{
    public class BizActionOutOnlyAsync : BizActionStatus, IBizActionOutOnlyAsync
    {
        public async Task<BizDataOut> BizActionAsync()
        {
            Message = "All Ok";
            return new BizDataOut("Result");
        }
    }
}
