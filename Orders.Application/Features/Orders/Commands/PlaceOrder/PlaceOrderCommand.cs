using System.Collections.Generic;
using MediatR;
using Orders.Application.Features.Orders.Queries.GetOrders;
using Orders.Domain.Entities;

namespace Orders.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommand : IRequest<OrdersVm>
    {
        // Order related details
        public virtual IEnumerable<ProductsVM> Products { get; set; }
        public decimal TotalPrice { get; set; }
        //public string Status { get; set; }

        // User related details
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Payment details
        public PaymentDetails PaymentDetails { get; set; }
    }
}
