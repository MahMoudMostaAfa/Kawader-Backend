using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces.Auth;

public interface ITokenProvider
{
  Task<Result<string>> GenerateTokenAsync(string userId);
  Result<string> GenerateRefreshTokenAsync();

}