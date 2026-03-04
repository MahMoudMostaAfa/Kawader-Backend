using FluentValidation;

namespace Kawadar.Application.Features.Admins.Commands.AddClaim
{
    public class AddClaimCommandValidator : AbstractValidator<AddClaimCommand>
    {
        public AddClaimCommandValidator()
        {
            RuleFor(x => x.userName).NotNull().WithMessage("UserName is required")
                .NotEmpty().WithMessage("UserName can't be empty");

            RuleFor(x => x.permission).NotNull().WithMessage("Permission is required")
                .NotEmpty().WithMessage("Permission can't be null");
        }
    }
}
