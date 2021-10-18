using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Contracts.Persistence;
using Orders.Application.Features.Orders.Common;
using Orders.Application.Features.Orders.Queries.GetOrders;
using Orders.Domain.Common;
using Orders.Payment;

namespace Orders.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OrdersVm>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PlaceOrderCommandHandler> _logger;
        private readonly rpcOrders.rpcOrdersClient _rpcOrdersServiceClient;

        public PlaceOrderCommandHandler(
            IOrderRepository orderRepository,
            IMapper mapper, 
            ILogger<PlaceOrderCommandHandler> logger,
            rpcOrders.rpcOrdersClient rpcOrdersServiceClient)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rpcOrdersServiceClient = rpcOrdersServiceClient ?? throw new ArgumentNullException(nameof(rpcOrdersServiceClient));
        }

        public async Task<OrdersVm> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            // transaction
            using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var orderEntity = _mapper.Map<Domain.Entities.Order>(request);
                orderEntity.Status = OrderStatus.New;
                orderEntity.CreatedAt = DateTime.UtcNow;
                var newOrder = await _orderRepository.AddAsync(orderEntity);

                var payment = await MakePayment.PayAsync(request.PaymentDetails);

                if (payment == "success") _logger.LogInformation($"Order {newOrder.Id} is successfully created.");
                else throw new Exception("Payment Unsuccessful.");

                var rpcReq = new rpcOrderAvailableReqest
                {
                    NewOrder = new rpcOrder
                    {
                        OrderNumber = newOrder.Id,
                        Amount = double.Parse(@$"{newOrder.TotalPrice}"),
                        CustomerName = $@"{newOrder.FirstName} {newOrder.LastName}",
                        CustomerPhone = newOrder.Phone,
                        Date = newOrder.CreatedAt.ToString("t", CultureInfo.CreateSpecificCulture("en-US")),
                        Location = newOrder.Address,
                        Status = newOrder.Status.mapOrderStatusToRpcClient(),
                    }
                };
                rpcReq.Products.AddRange(newOrder.Products.mapOrderProductsCollectionToRpcClient());

                var res = await _rpcOrdersServiceClient.OrderAvailableAsync(rpcReq, cancellationToken: cancellationToken);

                if (!res.Succeeded)
                    throw new Exception(res.ErrorMessage);
                // data will not be affected until complete operation is called. Any exception would rollback the transaction.
                scope.Complete();
                //return newOrder.Id;
                return _mapper.Map<OrdersVm>(newOrder);
            }
            throw new Exception("There was an error while placing the order!");
        }
    }
}
