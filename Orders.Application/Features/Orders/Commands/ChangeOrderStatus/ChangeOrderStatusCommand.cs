using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Orders.Domain.Common;

namespace Orders.Application.Features.Orders.Commands.ChangeOrderStatus
{
    public class ChangeOrderStatusCommand : IRequest
    {
        public int Id { get; set; }
        public OrderStatus Status { get; set; }
    }
}
