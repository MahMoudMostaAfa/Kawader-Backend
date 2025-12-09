using FluentValidation;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateProject
{
    public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
    {
        public CreateProjectCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Project title can't be Empty")
                .MaximumLength(50).WithMessage("the title length can't exceed 50 characters");

            RuleFor(x => x.Description).NotEmpty().WithMessage("Project Description can't be Empty")
                .MaximumLength(300).WithMessage("Project description can't exceed 300 characters");
        }
    }
}
