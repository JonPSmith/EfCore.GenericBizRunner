// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]

namespace TestBizLayer.BizDTOs
{
    public class BizDataOut
    {
        internal BizDataOut() {}

        public BizDataOut(string output)
        {
            Output = output;
        }

        public string Output { get; set; }
    }
}
