// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace TestBizLayer.DbForTransactions
{
    public class UniqueEntity
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string UniqueString { get; set; }
    }
}