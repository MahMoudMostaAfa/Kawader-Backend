
using System.Security.Claims;

namespace Kawadar.Application.Common.Interfaces.Auth;

public interface IUser
{
  string? Id { get; }
  List<string> Claims { get; }

}