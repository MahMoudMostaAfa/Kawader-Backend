using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ChangePassword;

public record ChangePasswordCommand(
  string CurrentPassword,
  string NewPassword
) : IRequest<Result<Updated>>;