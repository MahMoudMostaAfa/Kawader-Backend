using Kawadar.Domain.Common;


namespace Kawadar.Domain.UserProfiles.Events;

public class CreatedUserEvent : DomainEvent
{
  public string UserId { get; }
  public string Email { get; }
  public string FirstName { get; }

  public CreatedUserEvent(string userId, string email, string firstName)
  {
    UserId = userId;
    Email = email;
    FirstName = firstName;
  }
};