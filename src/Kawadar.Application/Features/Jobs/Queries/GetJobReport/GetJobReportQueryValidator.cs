using FluentValidation;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobReport
{
    public class GetJobReportQueryValidator : AbstractValidator<GetJobReportQuery>
    {
        public GetJobReportQueryValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Job Report Id is required")
                .NotEmpty().WithMessage("Job Report Id can't be empty");
        }
    }
}
