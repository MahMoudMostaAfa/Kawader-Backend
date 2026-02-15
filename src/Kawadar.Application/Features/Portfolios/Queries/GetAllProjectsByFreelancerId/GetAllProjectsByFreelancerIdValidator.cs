using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId
{
    public class GetAllProjectsByFreelancerIdValidator : AbstractValidator<GetAllProjectsByFreelancerIdQuery>
    {
        public GetAllProjectsByFreelancerIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Freelacner Id is Required")
                .NotEqual(Guid.Empty).WithMessage("Freelancer Id can't be Empty");
        }
    }
}
