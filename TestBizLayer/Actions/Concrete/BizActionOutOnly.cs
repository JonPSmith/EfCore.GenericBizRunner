// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionOutOnly : BizActionStatus, IBizActionOutOnly
    {
        public BizDataOut BizAction()
        {
            Message = "All Ok";
            return new BizDataOut("Result");
        }
    }
}
