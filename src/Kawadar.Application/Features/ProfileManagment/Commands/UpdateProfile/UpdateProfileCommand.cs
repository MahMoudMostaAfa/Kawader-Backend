using Kawadar.Domain.Common.Results;
using Kawadar.Domain.UserProfiles.Enums;
using MediatR;

namespace Kawadar.Application.Features.ProfileManagment.Commands.UpdateProfile;


public record UpdateProfileCommand(
  string? FirstName,
  string? LastName,
  string? Title,
  string? Bio,
  ExperienceYear? ExperienceYear,
  bool? IsAvailable,
  ProfileType? ProfileType,
  string? PhoneNumber
) : IRequest<Result<Updated>>;