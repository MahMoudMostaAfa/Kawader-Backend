using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;

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

            RuleFor(x => x.ProjectImage).Must(FileName => ExtensionValidator.ValidExtension(FileName.FileName, Extensions.AllowedImageExtensions))
                .When(File => File.ProjectImage is not null).WithMessage("Alllowed file extensions are png, jpg and jpeg");

            RuleFor(x => x.ProjectImage.Length).LessThanOrEqualTo(10 * 1024 * 1024).When(x => x.ProjectImage is not null)
                .WithMessage("Maximum File length is 10 MB");
        }
    }
}
