using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Orders.Application.Features.Orders.Queries.GetOrders;

namespace Orders.Application.Features.Orders.Queries.GetDailyOrders
{
    public class GetDailyOrdersQuery : IRequest<List<OrdersVm>>
    {
    }
}
