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

namespace Orders.Application.Features.Orders.Queries.GetDailyOrders
{
    public class GetDailyOrdersQueryHandler : IRequestHandler<GetDailyOrdersQuery, List<OrdersVm>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public GetDailyOrdersQueryHandler(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<OrdersVm>> Handle(GetDailyOrdersQuery request, CancellationToken cancellationToken)
        {
            var orderList = await _orderRepository.GetDailyOrders();
            return _mapper.Map<List<OrdersVm>>(orderList);
        }
    }
}
