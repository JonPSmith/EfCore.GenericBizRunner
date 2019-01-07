// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.ObjectModel;
using GenericBizRunner;

namespace TestBizLayer.Actions.Concrete
{
    public class BizActionGenericInOnly : BizActionStatus, IBizActionGenericInOnly
    {
        /// <summary>
        /// This is used to check that Generic inputs can be used in a direct approach
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public void BizAction(Collection<int> inputData)
        {
            
        }
    }
}
