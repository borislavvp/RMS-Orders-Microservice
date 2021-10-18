using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orders.Domain.Common;
using Orders.Domain.Entities;

namespace Orders.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetPreconfiguredOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed database associated with context {DbContextName}", typeof(OrderContext).Name);
            }
        }

        private static IEnumerable<Order> GetPreconfiguredOrders()
        {
            List<Product> products = new List<Product>{
                new Product
                {
                    Name = "Best Chorba",
                    Ingredients = "water, oil, beans, salt",
                    Price = (decimal) 4.50,
                    Quantity = 2,
                    ImageUrl = "https://media-cdn.tripadvisor.com/media/photo-s/1a/81/fd/8d/tortilla-with-chicken.jpg"
                },
                new Product
                {
                    Name = "Moussaka",
                    Ingredients = "potato, meat, salt, tomato, oil",
                    Price = (decimal)7.50,
                    Quantity = 1,
                    ImageUrl = "https://media-cdn.tripadvisor.com/media/photo-s/1a/81/fd/8d/tortilla-with-chicken.jpg"
                }
            };
            IEnumerable<Product> enumList = products;

            return new List<Order>
            {
                new Order() {
                    Products = enumList,
                    TotalPrice = (decimal) 22.75,
                    Status = OrderStatus.New,
                    UserId = "123",
                    FirstName = "Haralambos",
                    LastName = "Mambos",
                    Phone = "+31 6 1109 6663",
                    Address = "Eindhoven, Street 20",
                    CreatedAt =  DateTime.UtcNow
        }
            };
        }
    }
}
