using System;
using System.Collections.Generic;
using MediatR;
using Orders.Application.Features.Orders.Queries.GetOrders;

namespace Orders.Application.Features.Orders.Queries.GetOrdersPerCustomer
{
    public class GetOrdersPerCustomerQuery : IRequest<List<OrdersVm>>
    {
        public string UserId { get; set; }

        public GetOrdersPerCustomerQuery(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
