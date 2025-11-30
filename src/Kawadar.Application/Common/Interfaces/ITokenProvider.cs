using Kawadar.Domain.Common.Results;

namespace Kawadar.Application.Common.Interfaces;

public interface ITokenProvider
{
  Task<Result<string>> GenerateTokenAsync(string userId);

}