// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BizLogic.Orders;
using DataLayer.EfClasses;
using GenericBizRunner;
using Microsoft.EntityFrameworkCore;

namespace ServiceLayer.OrderServices
{
    public class GenericBizChangeDeliveryDto : GenericActionToBizDto<ChangeDeliverDto, GenericBizChangeDeliveryDto>
    {

        private List<string> _bookTitles;

        public int OrderId { get; set; }

        public string UserId { get; set; }

        public DateTime NewDeliveryDate { get; set; }

        //---------------------------------------------
        //Presentation layer items

        public string OrderNumber => $"SO{OrderId:D6}";

        public DateTime DateOrderedUtc { get; private set; }

        public DateTime OriginalDeliveryDate { get; private set; }

        public ImmutableList<string> BookTitles => _bookTitles.ToImmutableList();

        protected override void SetupSecondaryData(DbContext db, IBizActionStatus status)
        {
            if (OrderId == 0)
                throw new InvalidOperationException("You must set the OrderId before you call SetupSecondaryData");
            if (UserId == null)
                throw new InvalidOperationException("You must set the UserId before you call SetupSecondaryData");

            var order = db.Set<Order>()
                .Include(x => x.LineItems).ThenInclude(x => x.ChosenBook)
                .SingleOrDefault(x => x.OrderId == OrderId);

            if (order == null)
            {
                status.AddError("Sorry, I could not find the order you asked for.");
                //Log possible hacking 
                return;
            }

            DateOrderedUtc = order.DateOrderedUtc;
            OriginalDeliveryDate = order.ExpectedDeliveryDate;
            _bookTitles = order.LineItems.Select(x => x.ChosenBook.Title).ToList();
        }
    }
}