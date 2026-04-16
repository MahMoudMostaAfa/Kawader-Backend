using FluentValidation;

namespace Kawadar.Application.Features.Notifications.Queries.GetUserNotifications;


public class GetUserNotificationsQueryValidator : AbstractValidator<GetUserNotificationsQuery>
{
  public GetUserNotificationsQueryValidator()
  {
    RuleFor(x => x.Page).GreaterThan(0).WithMessage("Page number must be greater than 0.");
    RuleFor(x => x.PageSize).GreaterThan(0).WithMessage("Page size must be greater than 0.");
  }
}