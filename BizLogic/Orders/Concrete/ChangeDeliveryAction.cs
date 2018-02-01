// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using BizDbAccess.Orders;
using GenericBizRunner;

namespace BizLogic.Orders.Concrete
{
    public class ChangeDeliveryAction : BizActionStatus, IChangeDeliverAction
    {
        private readonly IChangeDeliverDbAccess _dbAccess;

        public ChangeDeliveryAction(IChangeDeliverDbAccess dbAccess)
        {
            _dbAccess = dbAccess;
        }

        public void BizAction(BizChangeDeliverDto dto)
        {
            var order = _dbAccess.GetOrder(dto.OrderId);
            if (order == null)
                throw new NullReferenceException("Could not find the order. Someone entering illegal ids?");


            
            Message = $"Your new delivery date is {dto.NewDeliveryDate.ToShortDateString()}.";
        }
    }
}