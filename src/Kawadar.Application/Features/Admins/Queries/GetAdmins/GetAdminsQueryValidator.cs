using FluentValidation;

namespace Kawadar.Application.Features.Admins.Queries.GetAdmins
{
    public class GetAdminsQueryValidator : AbstractValidator<GetAdminsQuery>
    {
        public GetAdminsQueryValidator()
        {
            RuleFor(x => x.page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.sortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");

        }
    }
}
