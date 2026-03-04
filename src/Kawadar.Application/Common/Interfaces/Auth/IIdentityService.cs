namespace Kawadar.Application.Common.Interfaces.Auth;

using Kawadar.Application.Common.Models;
using Kawadar.Domain.Common.Results;

public interface IIdentityService
{
  Task<Result<UserDto>> RegisterAsync(string email, string userName, string password);
  Task<Result<UserDto>> LoginAsync(string email, string password);
  Task<Result<Success>> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
  Task<Result<Success>> ResetPasswordAsync(string userId, string token, string newPassword);
  Task<Result<string>> GeneratePasswordResetTokenAsync(string userId);
  Task<Result<Success>> ConfirmEmailAsync(string userId, string token);
  Task<Result<string>> GenerateEmailConfirmationTokenAsync(string userId);
  Task<Result<Success>> UpdateUserAsync(string userId, string? email, string? userName);
  Task<Result<Success>> DeleteUserAsync(string userId);
  Task<Result<UserDto>> GetUserByIdAsync(string userId);
  Task<Result<UserDto>> GetUserByEmailAsync(string email);
  Task<Result<bool>> IsInRoleAsync(string userId, string role);
  Task<Result<Success>> AddToRoleAsync(string userId, string role);
  Task<Result<Success>> RemoveFromRoleAsync(string userId, string role);
  Task<Result<List<string>>> GetUserRolesAsync(string userId);
  Task<Result<Success>> AddClaimAsync(string userId, string claimType, string claimValue);
  Task<Result<Success>> RemoveClaimAsync(string userId, string claimType, string claimValue);
  Task<Result<List<(string Type, string Value)>>> GetUserClaimsAsync(string userId);

  Task<Result<bool>> IsAvailableUserNameAsync(string userName);
  Task<Result<UserDto>> GetUserByUserNameAsync(string userName);

  Task<Result<string>> GenerateUserNameAsync(string firstName, string lastName);
  Task<Result<IEnumerable<UserDto>>> GetUsersByIds(IEnumerable<string> Ids);
}