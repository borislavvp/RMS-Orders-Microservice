using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Orders.Application.Features.Orders.Queries.GetOrders
{
    public class GetAllOrdersQuery : IRequest<List<OrdersVm>>
    {
    }
}
