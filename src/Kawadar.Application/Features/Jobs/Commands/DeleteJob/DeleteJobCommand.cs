using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.DeleteJob;

public record DeleteJobCommand(string Slug) : IRequest<Result<Deleted>>;
