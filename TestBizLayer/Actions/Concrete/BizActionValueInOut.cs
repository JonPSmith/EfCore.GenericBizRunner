// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericBizRunner;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionValueInOut : BizActionStatus, IBizActionValueInOut
    {
        public string BizAction(int num)
        {
            if (num >= 0)
            {
                Message = "All Ok";
            }
            else
            {
                AddError("Error");
            }
            return num.ToString();
        }
    }
}