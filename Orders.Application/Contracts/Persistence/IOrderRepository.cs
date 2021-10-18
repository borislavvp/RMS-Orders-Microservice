using System.Collections.Generic;
using System.Threading.Tasks;
using Orders.Domain.Common;
using Orders.Domain.Entities;

namespace Orders.Application.Contracts.Persistence
{
    public interface IOrderRepository : IAsyncRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserId(string userId);
        Task ChangeOrderStatus(int orderId, OrderStatus status);
        Task<IEnumerable<Order>> GetDailyOrders();
    }
}
