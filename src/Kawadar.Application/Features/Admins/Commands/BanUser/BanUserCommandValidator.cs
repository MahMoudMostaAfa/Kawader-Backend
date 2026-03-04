using FluentValidation;

namespace Kawadar.Application.Features.Admins.Commands.BanUser
{
    public class BanUserCommandValidator : AbstractValidator<BanUserCommand>
    {
        public BanUserCommandValidator()
        {
            RuleFor(x => x.userName).NotNull().WithMessage("UserName is required")
                .NotEmpty().WithMessage("UserName can't be empty");

            RuleFor(x => x.BannedUntil).NotNull().WithMessage("Banned until is required")
                .GreaterThan(DateTime.Now);
        }
    }
}
