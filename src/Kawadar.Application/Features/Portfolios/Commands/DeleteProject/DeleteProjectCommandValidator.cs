using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteProject
{
    public class DeleteProjectCommandValidator : AbstractValidator<DeleteProjectCommand>
    {
        public DeleteProjectCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Project Id is required")
                .NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");
        }
    }
}
