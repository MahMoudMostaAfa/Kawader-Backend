using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.ReportJob
{
    public class ReportJobCommandValidator : AbstractValidator<ReportJobCommand>
    {
        public ReportJobCommandValidator()
        {
            RuleFor(x => x.slug).NotNull().WithMessage("The job slug is required")
                .NotEmpty().WithMessage("The job slug can't be empty");

            RuleFor(x => x.content).NotNull().WithMessage("The report content is required")
                .NotEmpty().WithMessage("The report content can't be empty");

            RuleFor(x => x.reportType).IsInEnum();
        }
    }
}