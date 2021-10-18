using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Orders.Application.Contracts.Persistence;
using Orders.Domain.Common;
using Orders.Domain.Entities;
using Orders.Infrastructure.Persistence;

namespace Orders.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext) : base(dbContext)
        {
        }

        public async Task ChangeOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            order.Status = status;

            _dbContext.Orders.Attach(order);
            _dbContext.Entry(order).Property(x => x.Status).IsModified = true;

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetDailyOrders()
        {
            var todaysDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            var orderList = await _dbContext.Orders
                                    .Where(o => o.CreatedAt.Date == todaysDate.Date)
                                    .ToListAsync();
            return orderList;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserId(string userId)
        {
            var orderList = await _dbContext.Orders
                                    .Where(o => o.UserId == userId)
                                    .ToListAsync();
            return orderList;
        }
    }
}
