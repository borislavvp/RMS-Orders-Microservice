using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Orders.Application.Contracts.Persistence;
using Orders.Application.Features.Orders.Queries.GetOrders;

namespace Orders.Application.Features.Orders.Queries.GetSingleOrder
{
    public class GetSingleOrderQueryHandler : IRequestHandler<GetSingleOrderQuery, OrdersVm>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetSingleOrderQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<OrdersVm> Handle(GetSingleOrderQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderRepository.GetByIdAsync(request.Id);
            return _mapper.Map<OrdersVm>(order);
        }
    }
}
