using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawadar.Application.Features.Badges.Queries
{
    public class GetBadgeByIdQueryValidator: AbstractValidator<GetBadgeByIdQuery>
    {
        public GetBadgeByIdQueryValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Badge Id is Required")
                .Equal(Guid.Empty).WithMessage("Badge Id can't be empty");
        }
    }
}
