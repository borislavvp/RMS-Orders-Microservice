using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Features.Orders.Commands.DeleteOrder;
using Orders.Application.Features.Orders.Commands.PlaceOrder;
using Orders.Application.Features.Orders.Commands.UpdateOrder;
using Orders.Application.Features.Orders.Queries.GetOrders;
using Orders.Application.Features.Orders.Queries;
using Orders.Application.Features.Orders.Queries.GetSingleOrder;
using Orders.Application.Features.Orders.Queries.GetOrdersPerCustomer;
using Orders.Application.Features.Orders.Commands.ChangeOrderStatus;
using Orders.Application.Features.Orders.Queries.GetDailyOrders;

namespace Orders.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet(Name = "GetOrdersList")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetAllOrders(string userId)
        {
            if (userId != null) return Ok(await _mediator.Send(new GetOrdersPerCustomerQuery(userId)));
            else return Ok(await _mediator.Send(new GetAllOrdersQuery()));
        }

        [HttpGet("today", Name = "GetDailyOrders")]
        [ProducesResponseType(typeof(IEnumerable<OrdersVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<OrdersVm>>> GetDailyOrders()
        {
            return Ok(await _mediator.Send(new GetDailyOrdersQuery()));
        }

        [HttpGet("{id}", Name = "GetOrderById")]
        [ProducesResponseType(typeof(OrdersVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrdersVm>> GetOrderById(int id)
        {
            var query = new GetSingleOrderQuery() { Id = id };
            var order = await _mediator.Send(query);
            return Ok(order);
        }

        [HttpPost(Name = "PlaceOrder")]
        [ProducesResponseType(typeof(OrdersVm), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<OrdersVm>> PlaceOrder([FromBody] PlaceOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        //[HttpPut(Name = "UpdateOrder")]
        //[ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesDefaultResponseType]
        //public async Task<ActionResult> UpdateOrder([FromBody] UpdateOrderCommand command)
        //{
        //    await _mediator.Send(command);
        //    return NoContent();
        //}

        [HttpPatch(Name = "ChangeOrderStatus")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> ChangeOrderStatus([FromBody] ChangeOrderStatusCommand command)
        {
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}", Name = "DeleteOrder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand() { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}
