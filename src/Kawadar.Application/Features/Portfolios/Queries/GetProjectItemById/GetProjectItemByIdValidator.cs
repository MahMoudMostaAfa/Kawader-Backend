using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Application.Features.Portfolios.Queries.GetProjectItemById
{
    public class GetProjectItemByIdValidator : AbstractValidator<GetProjectItemByIdQuery>
    {
        public GetProjectItemByIdValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("ProjectItem Id is required")
                .NotEqual(Guid.Empty).WithMessage("ProjectItem Id can't be empty");
        }
    }
}
