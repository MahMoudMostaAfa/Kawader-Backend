using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand(string Email) : IRequest<Result<Success>>;