// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.BookServices
{
    public class AddRemovePromotionDto 
    {
        public int BookId { get; set; }
        public decimal OrgPrice { get; set; }
        public string Title { get; set; }

        public decimal ActualPrice { get; set; }
        //[Required(AllowEmptyStrings = false)]
        public string PromotionalText { get; set; }
    }

}