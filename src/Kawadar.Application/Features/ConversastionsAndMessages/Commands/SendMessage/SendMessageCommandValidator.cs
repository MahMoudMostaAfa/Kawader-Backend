using FluentValidation;
using Kawadar.Application.Common.ExtensionValidator;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Commands.SendMessage;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
  private const long MaxFileSizeBytes = 20 * 1024 * 1024; // 20 MB
  public SendMessageCommandValidator()
  {
    When(x => x.SenderId != null, () =>
    {
      RuleFor(x => x.SenderId).NotEmpty();
    });


    When(x => x.connectionId != null, () =>
    {
      RuleFor(x => x.connectionId).NotEmpty();
    });

    RuleFor(x => x.conversationId).NotEmpty();
    RuleFor(x => x.content).NotEmpty().MaximumLength(2000);

    When(x => x.replyToMessageId != null, () =>
    {
      RuleFor(x => x.replyToMessageId).NotEmpty();
    });

    When(x => x.AttachmentFiles != null, () =>
    {

      RuleForEach(x => x.AttachmentFiles).ChildRules(file =>
  {
    file.RuleFor(f => f.Length)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage($"Attachment file size must not exceed {MaxFileSizeBytes / (1024 * 1024)} MB.");

    file.RuleFor(f => f.FileName)
            .Must(name => ExtensionValidator.ValidExtension(name, Extensions.AllowedJobAttachmentExtensions))
            .WithMessage($"Allowed file types: {string.Join(", ", Extensions.AllowedJobAttachmentExtensions)}.");
  });
    }
    );
    When(x => x.AttachmentLinks != null, () =>
    {

      RuleForEach(x => x.AttachmentLinks).ChildRules(link =>
      {
        link.RuleFor(url => url)
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
                .WithMessage("External URL must be a valid absolute URL.");
      });

    });



  }
}