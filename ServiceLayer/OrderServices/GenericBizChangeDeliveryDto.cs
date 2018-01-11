// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using BizLogic.Orders;
using GenericBizRunner;

namespace ServiceLayer.OrderServices
{
    public class GenericBizChangeDeliveryDto : GenericActionToBizDto<ChangeDeliverDto, GenericBizChangeDeliveryDto>
    {
        public int OrderId { get; set; }

        public string UserId { get; set; }

        public DateTime NewDeliveryDate { get; set; }

        //---------------------------------------------
        //Presentation layer items

        public DateTime OriginalDeliveryDate { get; set; }


    }
}