using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orders.Domain.Entities;

namespace Orders.Application.Features.Orders.Queries.GetOrders
{
    public class OrdersVm
    {
        public int Id { get; protected set; }

        // Order related details
        public virtual IEnumerable<ProductsVM> Products { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public string CreatedAt { get; set; }

        // User related details
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
