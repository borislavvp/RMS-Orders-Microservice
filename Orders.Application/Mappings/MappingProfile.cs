using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Orders.Application.Features.Orders;
using Orders.Application.Features.Orders.Commands.PlaceOrder;
using Orders.Application.Features.Orders.Commands.UpdateOrder;
using Orders.Application.Features.Orders.Queries.GetOrders;
using Orders.Domain.Entities;

namespace Orders.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrdersVm>()
                .ForMember(v => v.CreatedAt,opt => opt.MapFrom(src => src.CreatedAt.ToString("t", CultureInfo.CreateSpecificCulture("en-US"))))
                .ReverseMap();
            CreateMap<Order, PlaceOrderCommand>().ReverseMap();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
            CreateMap<Product, ProductsVM>().ReverseMap();
        }
    }
}
