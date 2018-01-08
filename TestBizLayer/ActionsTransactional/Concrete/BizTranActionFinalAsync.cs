// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Threading.Tasks;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.ActionsTransactional.Concrete
{
    public class BizTranActionFinalAsync : BizTranActionBase, IBizTranActionFinalAsync
    {
        public BizTranActionFinalAsync()
            : base(3)
        {
        }

        public async Task<BizDataGuid> BizActionAsync(BizDataGuid input)
        {
            return BizAction(input);
        }
    }
}
