using FluentValidation;

namespace Kawadar.Application.Features.ProfileManagment.Queries.GetUserProfile;


public class UserProfileQueryValidator : AbstractValidator<GetUserProfileQuery>
{
  public UserProfileQueryValidator()
  {
    // No validation rules needed for this query
  }
}