// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsTransactional.Concrete
{
    public class BizTranActionSecondAsync : BizTranActionBase, IBizTranActionSecondAsync
    {
        public BizTranActionSecondAsync()
            : base(2)
        {
        }

        public async Task<BizDataGuid> BizActionAsync(BizDataGuid input)
        {
            return BizAction(input);
        }
    }
}
