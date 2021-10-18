using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Application.Features.Orders.Commands.PlaceOrder
{
    public class PaymentDetails
    {
        public string CardNumber { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string CVV { get; set; }
        public int Value { get; set; }
    }
}
