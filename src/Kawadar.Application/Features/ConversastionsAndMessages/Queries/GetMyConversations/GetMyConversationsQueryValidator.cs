using FluentValidation;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Queries.GetMyConversations;

public class GetMyConversationsQueryValidator : AbstractValidator<GetMyConversationsQuery>
{
  public GetMyConversationsQueryValidator()
  {
    RuleFor(x => x.pageNumber).GreaterThan(0).WithMessage("Page number must be greater than 0.");
    RuleFor(x => x.pageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.");
  }
}