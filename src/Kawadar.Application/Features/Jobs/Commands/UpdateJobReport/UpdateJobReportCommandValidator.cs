
using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Commands.UpdateJobReport
{
    public class UpdateJobReportCommandValidator : AbstractValidator<UpdateJobReportCommand>
    {
        public UpdateJobReportCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Job Report Id is Required")
                .NotEqual(Guid.Empty).WithMessage("Job Report Id can't be empty");

            RuleFor(x => x.reportStatus).IsInEnum();

            RuleFor(x => x.ActionTaken).NotEmpty().When(x => x.reportStatus == Domain.Jobs.JobReports.Enums.ReportStatus.Resolved);
        }
    }
}