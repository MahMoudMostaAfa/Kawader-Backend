using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobById;


public record GetJobByIdQuery(Guid JobId) : ICachedQuery
<Result<JobDetailsDto>>
{
  public string CacheKey => $"GetJobByIdQuery:{JobId:N}";

  public string[] Tags => new[] { $"{JobId}" };
}