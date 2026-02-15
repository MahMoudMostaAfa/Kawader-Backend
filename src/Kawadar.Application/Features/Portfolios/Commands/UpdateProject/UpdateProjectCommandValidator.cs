using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;


namespace Kawadar.Application.Features.Portfolios.Commands.UpdateProject
{
    public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
    {
        public UpdateProjectCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Project Id is required").
                NotEqual(Guid.Empty).WithMessage("Project Id can't be empty");


            RuleFor(x => x.Image.Length).LessThanOrEqualTo(10 * 1024 * 1024).WithMessage("File size can't exceed 10 MB");

            RuleFor(x => x.Image.FileName).Must(FileName => ExtensionValidator.ValidExtension(FileName, Extensions.AllowedImageExtensions))
                .WithMessage("The supported file extensions are jpg, jpeg and png");
        }
    }
}
