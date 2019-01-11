// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using GenericBizRunner;

namespace ServiceLayer.CheckoutServices
{
    public class OrderIdDto : GenericActionFromBizDto<Order, OrderIdDto>
    {
        public int OrderId { get; set; }
    }
}