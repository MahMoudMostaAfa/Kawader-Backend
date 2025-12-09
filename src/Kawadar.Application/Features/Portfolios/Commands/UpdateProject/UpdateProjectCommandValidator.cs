using FluentValidation;


namespace Kawadar.Application.Features.Portfolios.Commands.UpdateProject
{
    public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Project Id is required").
                NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");
        }
    }
}
