using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Orders.Application.Contracts.Persistence;
using Orders.Application.Exceptions;
using Orders.Application.Features.Orders.Common;
using Orders.Domain.Common;
using Orders.Domain.Entities;

namespace Orders.Application.Features.Orders.Commands.ChangeOrderStatus
{
    public class ChangeOrderStatusCommandHandler : IRequestHandler<ChangeOrderStatusCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ChangeOrderStatusCommandHandler> _logger;
        private readonly rpcOrders.rpcOrdersClient _rpcOrdersServiceClient;

        public ChangeOrderStatusCommandHandler(
            IOrderRepository orderRepository, 
            IMapper mapper, 
            ILogger<ChangeOrderStatusCommandHandler> logger,
            rpcOrders.rpcOrdersClient rpcOrdersServiceClient)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _rpcOrdersServiceClient = rpcOrdersServiceClient ?? throw new ArgumentNullException(nameof(rpcOrdersServiceClient));
        }

        public async Task<Unit> Handle(ChangeOrderStatusCommand request, CancellationToken cancellationToken)
        {
            using TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled);

            var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);

            if (orderToUpdate == null)
            {
                throw new NotFoundException(nameof(Order), request.Id);
            }

            await _orderRepository.ChangeOrderStatus(request.Id, request.Status);

            var rpcStatusChangedReq = new rpcOrderStatusChangedReqest
            {
                OrderNumber = orderToUpdate.Id,
                Status = orderToUpdate.Status.mapOrderStatusToRpcClient()
            };

            async Task<rpcRequestResult> informStatusChanged() =>
                await _rpcOrdersServiceClient.OrderStatusChangedAsync(rpcStatusChangedReq, cancellationToken: cancellationToken);

            if (request.Status.Equals(OrderStatus.Prepared))
            {
                var rpcReadyForPickupReq = new rpcOrderReadyForPickupReqest
                {
                    NewOrder = new rpcOrder
                    {
                        OrderNumber = orderToUpdate.Id,
                        Amount = double.Parse(@$"{orderToUpdate.TotalPrice}"),
                        CustomerName = $@"{orderToUpdate.FirstName} {orderToUpdate.LastName}",
                        CustomerPhone = orderToUpdate.Phone,
                        Date = orderToUpdate.CreatedAt.ToString("t", CultureInfo.CreateSpecificCulture("en-US")),
                        Location = orderToUpdate.Address,
                        Status = orderToUpdate.Status.mapOrderStatusToRpcClient(),
                    }
                };

                async Task<rpcRequestResult> informOrderReadyForPickup() =>
                    await _rpcOrdersServiceClient.OrderReadyForPickupAsync(rpcReadyForPickupReq, cancellationToken: cancellationToken);

                var results = await Task.WhenAll(Task.Run(() => informOrderReadyForPickup()), Task.Run(() => informStatusChanged()));
                foreach (var result in results)
                {
                    if (!result.Succeeded)
                        throw new Exception(result.ErrorMessage);
                }
            }
            else
            {
                var res = await informStatusChanged(); 
                if (!res.Succeeded)
                    throw new Exception(res.ErrorMessage);
            }

            _logger.LogInformation($"Order {orderToUpdate.Id} is {orderToUpdate.Status}");

            scope.Complete();

            return Unit.Value;
        }

    }
}
