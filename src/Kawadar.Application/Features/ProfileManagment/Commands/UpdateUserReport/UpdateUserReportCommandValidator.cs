using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateUserReport
{
    public class UpdateUserReportCommandValidator : AbstractValidator<UpdateUserReportCommand>
    {
        public UpdateUserReportCommandValidator()
        {
            RuleFor(x => x.reportId).NotNull().WithMessage("Report Id is required")
                .NotEqual(Guid.Empty).WithMessage("Report Id can't be empty");

            RuleFor(x => x.ReportStatus).IsInEnum();

            RuleFor(x => x.ActionTaken).NotEmpty().When(x => x.ReportStatus == Domain.Jobs.JobReports.Enums.ReportStatus.Resolved);
        }
    }
}
