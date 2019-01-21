// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using ServiceLayer.CheckoutServices;

namespace ServiceLayer.OrderServices
{
    public class OrderListDto
    {
        public int OrderId { get; set; }

        public DateTime DateOrderedUtc { get; set; }

        public DateTime ExpectedDeliveryDate { get; set; }

        public bool HasBeenDelivered { get; set; }

        public string OrderNumber => $"SO{OrderId:D6}";

        public IEnumerable<CheckoutItemDto> LineItems { get; set; }
    }
}