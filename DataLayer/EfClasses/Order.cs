// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using GenericBizRunner;

namespace DataLayer.EfClasses
{
    public class Order
    {
        private HashSet<LineItem> _lineItems;

        public int OrderId { get; private set; }

        public DateTime DateOrderedUtc { get; private set; }

        public DateTime ExpectedDeliveryDate { get; private set; }

        public bool HasBeenDelivered { get; private set; }

        /// <summary>
        /// In this simple example the cookie holds a GUID for everyone that 
        /// </summary>
        public string CustomerName { get; private set; }

        // relationships

        public IImmutableList<LineItem> LineItems => _lineItems.ToImmutableList();

        // Extra columns not used by EF

        public string OrderNumber => $"SO{OrderId:D6}";

        /// <summary>
        /// Use by EF Core
        /// </summary>
        internal Order()
        {
        }

        public Order(string customerName, DateTime expectedDeliveryDate, IEnumerable<LineItem> lineItems)
        {
            CustomerName = customerName;
            ExpectedDeliveryDate = expectedDeliveryDate;

            DateOrderedUtc = DateTime.UtcNow;
            HasBeenDelivered = expectedDeliveryDate < DateTime.Today;
            _lineItems = new HashSet<LineItem>(lineItems);
        }

        //----------------------------------------------------
        //aggregate methods

        public IGenericErrorHandler ChangeDeliveryDate(string userId, DateTime newDeliveryDate)
        {
            var status = new GenericErrorHandler();
            if (CustomerName != userId)
            {
                status.AddError("I'm sorry, but that order does not belong to you");
                //Log a security issue
                return status;
            }

            if (HasBeenDelivered)
            {
                status.AddError("I'm sorry, but that order has been delivered.");
                return status;
            }

            if (newDeliveryDate < DateTime.Today.AddDays(1))
            {
                status.AddError("I'm sorry, we cannot get the order to you that quickly. Please choose a new date.", "NewDeliveryDate");
                return status;
            }

            if (newDeliveryDate.DayOfWeek == DayOfWeek.Sunday)
            {
                status.AddError("I'm sorry, we don't deliver on Sunday. Please choose a new date.", "NewDeliveryDate");
                return status;
            }

            //All Ok
            ExpectedDeliveryDate = newDeliveryDate;
            return status;
        }
    }
}