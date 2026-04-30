using FluentValidation;

namespace Kawadar.Application.Features.Contracts.Disbutes.Queries.GetDisbutes
{
    public class GetDisbutesQueryValidator : AbstractValidator<GetDisbutesQuery>
    {
        public GetDisbutesQueryValidator()
        {
            RuleFor(x => x.status).IsInEnum().When(x => x.status is not null);
            RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.SortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");
        }
    }
}
