using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateView
{
    public class CreateViewValidator : AbstractValidator<CreateViewCommand>
    {
        public CreateViewValidator()
        {
            RuleFor(x => x.ProjectId).NotEmpty().WithMessage("Project Id is required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");
        }
    }
}
