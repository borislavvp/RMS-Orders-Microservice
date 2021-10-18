using Google.Protobuf.Collections;
using Orders.Domain.Common;
using Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.Application.Features.Orders.Common
{
    public static class RPCHelpers
    {
        public static rpcOrderStatus mapOrderStatusToRpcClient(this OrderStatus status)
        {
            return status switch
            {
                OrderStatus.New => rpcOrderStatus.New,
                OrderStatus.Preparing => rpcOrderStatus.Preparing,
                OrderStatus.Prepared => rpcOrderStatus.Prepared,
                OrderStatus.Delivering => rpcOrderStatus.Delivering,
                OrderStatus.Delivered => rpcOrderStatus.Delivered,
                _ => rpcOrderStatus.Unknown,
            };
        }
        public static RepeatedField<rpcOrderProduct> mapOrderProductsCollectionToRpcClient(this IEnumerable<Product> products)
        {
            var res = new RepeatedField<rpcOrderProduct>();
            products.ToList().ForEach(p => res.Add(p.mapOrderProductToRpcClient()));
            return res;
        } 
        
        public static rpcOrderProduct mapOrderProductToRpcClient(this Product product)
        {
            return new rpcOrderProduct
            {
               ImageUrl = product.ImageUrl,
               Ingredients = product.Ingredients,
               Name = product.Name,
               Price = Double.Parse($@"{product.Price}"),
               ProductId = product.Id,
               Quantity = product.Quantity
            };
        }
    }
}
