using FluentValidation;
using Kawadar.Application.Features.Portfolios.DTOs;

namespace Kawadar.Application.Features.Portfolios.Commands.OrderProjectItems
{
    public class OrderProjectItemsValidator : AbstractValidator<OrderProjectItemsCommand>
    {
        public OrderProjectItemsValidator()
        {
            RuleFor(x => x.Order).NotNull().WithMessage("Order List can't be null")
                .NotEmpty().WithMessage("Order List must have at least one item");

            RuleForEach(x => x.Order).ChildRules(item =>
            {
                item.RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");
            });

            RuleFor(x => x.Order).Must(HaveUniqueDisplayOrders).WithMessage("Duplicate display orders are not allowed");

            RuleFor(x => x.Order).Must(HaveUniqueIds).WithMessage("Duplicate Projects are not allowed");
        }

        private bool HaveUniqueIds(List<ItemOrderDTO> order)
        {
            return order.Select(x => x.Id).Distinct().Count() == order.Count;
        }

        private bool HaveUniqueDisplayOrders(List<ItemOrderDTO> order)
        {
            return order.Select(x => x.DisplayOrder).Distinct().Count() == order.Count;
        }
    }
}
