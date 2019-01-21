// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace ServiceLayer.BookServices
{
    public class ChangePubDateDto
    {
        public int BookId { get; set; }         
        public string Title { get; set; }       
        [DataType(DataType.Date)]               
        public DateTime PublishedOn { get; set; }
    }
}