// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AutoMapper;
using BizLogic.Orders;
using DataLayer.EfClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExampleWebApp.Models
{
    public class WebChangeDeliveryDto : Profile
    {
        public WebChangeDeliveryDto()
        {
            CreateMap<WebChangeDeliveryDto, BizChangeDeliverDto>();
        }

        [HiddenInput(DisplayValue = false)]
        public int OrderId { get; set; }

        [HiddenInput(DisplayValue = false)]
        public string UserId { get; set; }

        public DateTime NewDeliveryDate { get; set; }

        //---------------------------------------------
        //Presentation layer items

        public string OrderNumber => $"SO{OrderId:D6}";

        public DateTime DateOrderedUtc { get; private set; }

        public DateTime OriginalDeliveryDate { get; private set; }

        public List<string> BookTitles { get; private set; }

        public SelectList PossibleDeliveryDates { get; private set; }

        protected  void SetupSecondaryData(DbContext db)
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
                throw new InvalidOperationException("I could not find the order you asked for.");
            }

            DateOrderedUtc = order.DateOrderedUtc;
            OriginalDeliveryDate = order.ExpectedDeliveryDate;
            NewDeliveryDate = OriginalDeliveryDate < DateTime.Today
                ? DateTime.Today
                : OriginalDeliveryDate;
            BookTitles = order.LineItems.Select(x => x.ChosenBook.Title).ToList();
            PossibleDeliveryDates = new SelectList(FormPossibleDeliveryDates(DateTime.Today));
            var selected = PossibleDeliveryDates.FirstOrDefault(x => x.Text == NewDeliveryDate.ToString("d"));
            if (selected != null)
                selected.Selected = true;
        }

        private IEnumerable<string> FormPossibleDeliveryDates(DateTime startDate)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return startDate.ToString("d");
                startDate = startDate.AddDays(1);
            }
        }
    }
}