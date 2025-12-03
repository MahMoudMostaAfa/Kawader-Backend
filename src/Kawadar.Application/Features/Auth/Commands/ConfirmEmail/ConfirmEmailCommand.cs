using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result<Success>>;