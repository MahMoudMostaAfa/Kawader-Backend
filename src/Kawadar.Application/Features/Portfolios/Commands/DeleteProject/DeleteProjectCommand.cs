using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.DeleteProject
{
    public record DeleteProjectCommand(Guid Id) : IRequest<Result<Deleted>>;
}
