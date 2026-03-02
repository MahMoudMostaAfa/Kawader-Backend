using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;

namespace Kawadar.Application.Features.Jobs.Commands.AddJobAttachment;

public class AddJobAttachmentCommandValidator : AbstractValidator<AddJobAttachmentCommand>
{
  private const long MaxFileSizeBytes = 20 * 1024 * 1024; // 20 MB

  public AddJobAttachmentCommandValidator()
  {
    RuleFor(x => x.Slug).NotEmpty().MaximumLength(100);

    RuleFor(x => x)
      .Must(x => x.File is not null || !string.IsNullOrWhiteSpace(x.ExternalUrl))
      .WithMessage("Either a file or an external URL must be provided.");

    RuleFor(x => x)
      .Must(x => x.File is null || string.IsNullOrWhiteSpace(x.ExternalUrl))
      .WithMessage("Provide either a file or an external URL, not both.");

    When(x => x.File is not null, () =>
    {
      RuleFor(x => x.File!.Length)
        .LessThanOrEqualTo(MaxFileSizeBytes)
        .WithMessage($"Attachment file size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB.");

      RuleFor(x => x.File!.FileName)
        .Must(name => ExtensionValidator.ValidExtension(name, Extensions.AllowedJobAttachmentExtensions))
        .WithMessage($"Allowed file types: {string.Join(", ", Extensions.AllowedJobAttachmentExtensions)}.");
    });

    When(x => !string.IsNullOrWhiteSpace(x.ExternalUrl), () =>
    {
      RuleFor(x => x.ExternalUrl!)
        .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
        .WithMessage("External URL must be a valid absolute URL.");
    });
  }
}
