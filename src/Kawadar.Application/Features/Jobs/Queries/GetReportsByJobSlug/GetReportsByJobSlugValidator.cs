using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug
{
    public class GetReportsByJobSlugValidator : AbstractValidator<GetReportsByJobSlugQuery>
    {
        public GetReportsByJobSlugValidator()
        {
            RuleFor(x => x.JobSlug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.");
        }
    }
}
