using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Queries.GetAllProjectsByFreelancerId
{
    public class GetAllProjectsByFreelancerUserNameValidator : AbstractValidator<GetAllProjectsByFreelancerUserNameQuery>
    {
        public GetAllProjectsByFreelancerUserNameValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("Freelacner UserName is Required")
                .NotEmpty().WithMessage("Freelancer UserName can't be Empty");
        }
    }
}
