using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Commands.ReportUser
{
    public class ReportUserCommandValidator: AbstractValidator<ReportUserCommand>
    {
        public ReportUserCommandValidator()
        {
            RuleFor(x => x.ReportedUserName).NotNull().WithMessage("Reported UserName Is needed")
                .NotEmpty().WithMessage("Reported UserName can't be empty")
                .MaximumLength(30).WithMessage("Maximum length for the username exceeded");

            RuleFor(x => x.reportType).IsInEnum();

            RuleFor(x => x.content).NotNull().WithMessage("Report content is required")
                .NotEmpty().WithMessage("Report content can't be empty")
                .MaximumLength(500).WithMessage("Maximum lenth exceeded");
        }
    }
}
