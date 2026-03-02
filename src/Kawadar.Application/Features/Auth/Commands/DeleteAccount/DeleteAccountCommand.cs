using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Auth.Commands.DeleteAccount;

public record DeleteAccountCommand() : IRequest<Result<Deleted>>;
