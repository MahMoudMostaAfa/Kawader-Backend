using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ResetPassword;

public record ResetPasswordCommand(string UserId, string Token, string NewPassword) : IRequest<Result<Success>>;