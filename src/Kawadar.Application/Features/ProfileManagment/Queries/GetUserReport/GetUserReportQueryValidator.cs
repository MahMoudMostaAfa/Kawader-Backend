using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserReport
{
    public class GetUserReportQueryValidator : AbstractValidator<GetUserReportQuery>
    {
        public GetUserReportQueryValidator()
        {
            RuleFor(x => x.reportId).NotNull().WithMessage("The Report Id is required")
                .NotEmpty().WithMessage("The Report Id can't be empty");
        }
    }
}
