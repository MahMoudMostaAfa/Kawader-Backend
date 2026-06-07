using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.SavedJobs.Queries.GetSavedJobsByUser;


public record GetSavedJobsByUserQuery(int PageNumber, int PageSize) : IRequest<Result<PaginatedList<JobSummaryDto>>>;