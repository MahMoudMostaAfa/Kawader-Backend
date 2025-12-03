using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ForgetPassword;

public record ForgetPasswordCommand(string Email) : IRequest<Result<Success>>;
