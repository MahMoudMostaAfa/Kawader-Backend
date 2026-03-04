using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.CreateAdmin
{
    public record CreateAdminCommand(string FirstName, string LastName, string Email, string Password) : IRequest<Result<Success>>;
}
