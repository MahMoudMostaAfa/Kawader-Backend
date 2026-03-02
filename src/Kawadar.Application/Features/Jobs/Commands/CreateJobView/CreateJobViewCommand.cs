using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Commands.CreateJobView;

public record CreateJobViewCommand(string Slug) : IRequest<Result<Created>>;
