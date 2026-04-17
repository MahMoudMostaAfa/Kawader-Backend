using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Commands.RemoveSavedJob;


public record RemoveSavedJobCommand(Guid JobId) : IRequest<Result<Deleted>>;