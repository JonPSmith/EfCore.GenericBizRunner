// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;

namespace BizLogic.Orders
{
    public class ChangeDeliverDto
    {
        public int OrderId { get; set; }

        public string UserId { get; set; }

        public DateTime NewDeliveryDate { get; set; }
    }
}