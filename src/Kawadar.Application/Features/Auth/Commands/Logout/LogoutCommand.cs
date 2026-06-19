using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.Logout;

public record LogoutCommand : IRequest<Result<Success>>;