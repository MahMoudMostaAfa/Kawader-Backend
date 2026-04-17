using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Commands.AddSavedJob;


public record AddSavedJobCommand(Guid JobId) : IRequest<Result<Created>>;