// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;
using TestBizLayer.BizDTOs;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionNestedOutOnly : BizActionStatus, IBizActionNestedOutOnly
    {
        public NestedBizDataOut BizAction()
        {
            Message = "All Ok";
            return new NestedBizDataOut
            {
                Output = "Test",
                ChildData = new NestedBizDataOutChild
                {
                    ChildInt = 123,
                    ChildString = "Nested"
                }
            };
        }
    }
}
