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

            if (order.CustomerName != dto.UserId)
            {
                AddError("I'm sorry, but that order does not belong to you");
                //Log a security issue
                return;
            }

            if (order.HasBeenDelivered)
            {
                AddError("I'm sorry, but that order has been delivered.");
                return;
            }

            if (dto.NewDeliveryDate < DateTime.Today.AddDays(1))
            {
                AddError("I'm sorry, we cannot get the order to you that quickly. Please choose a new date.", "NewDeliveryDate");
                return;
            }

            if (dto.NewDeliveryDate.DayOfWeek == DayOfWeek.Sunday)
            {
                AddError("I'm sorry, we don't deliver on Sunday. Please choose a new date.", "NewDeliveryDate");
                return;
            }

            //All Ok
            order.ExpectedDeliveryDate = dto.NewDeliveryDate;
            Message = $"Your new delivery date is {dto.NewDeliveryDate.ToShortDateString()}.";
        }
    }
}