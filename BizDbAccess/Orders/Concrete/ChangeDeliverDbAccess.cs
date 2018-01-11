// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using DataLayer.EfClasses;
using DataLayer.EfCode;

namespace BizDbAccess.Orders.Concrete
{
    public class ChangeDeliverDbAccess : IChangeDeliverDbAccess
    {
        private readonly OrderDbContext _context;

        public ChangeDeliverDbAccess(OrderDbContext context)
        {
            _context = context;
        }

        public Order GetOrder(int orderId)
        {
            return _context.Find<Order>(orderId);
        }
    }
}