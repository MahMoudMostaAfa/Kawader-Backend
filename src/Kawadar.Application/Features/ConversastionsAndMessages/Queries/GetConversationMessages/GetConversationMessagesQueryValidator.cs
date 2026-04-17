using FluentValidation;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetConversationMessages;

public class GetConversationMessagesQueryValidator : AbstractValidator<GetConversationMessagesQuery>
{
  public GetConversationMessagesQueryValidator()
  {
    RuleFor(x => x.conversationId).NotEmpty();
    RuleFor(x => x.PageNumber).GreaterThan(0);
    RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);


  }
}