using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Orders.Application.Features.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("{UserId} is required.").NotNull();
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("{FirstName} is required.").NotNull();
            RuleFor(x => x.LastName).NotEmpty().WithMessage("{LastName} is required.").NotNull();
            RuleFor(x => x.Phone).NotEmpty().WithMessage("{Phone} is required.").NotNull();
            RuleFor(x => x.Address).NotEmpty().WithMessage("{Address} is required.").NotNull();
            RuleFor(x => x.TotalPrice).NotEmpty().WithMessage("{TotalPrice} is required.")
                .GreaterThan(0).WithMessage("{TotalPrice} should be greater than 0.").NotNull();
        }
    }
}
