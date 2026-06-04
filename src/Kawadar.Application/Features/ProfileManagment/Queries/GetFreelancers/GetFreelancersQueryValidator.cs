using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetFreelancers
{
    public class GetFreelancersQueryValidator : AbstractValidator<GetFreelancersQuery>
    {
        public GetFreelancersQueryValidator()
        {
            RuleFor(x => x.page).GreaterThanOrEqualTo(1);
            RuleFor(x => x.pageSize).GreaterThanOrEqualTo(1).LessThanOrEqualTo(50);

            RuleFor(x => x.sortBy)
              .Must(s => s is "newest" or "oldest")
              .WithMessage("SortBy must be 'newest' or 'oldest'.");

            RuleFor(x => x.specilizationId).NotEqual(Guid.Empty).When(x => x.specilizationId is not null);
            RuleFor(x => x.ExperienceYear).IsInEnum().When(x => x.ExperienceYear is not null);

            RuleFor(x => x.averageRating).GreaterThan(0.0f).When(x => x.averageRating is not null).LessThanOrEqualTo(5.0f).When(x => x.averageRating is not null);
        }
    }
}
