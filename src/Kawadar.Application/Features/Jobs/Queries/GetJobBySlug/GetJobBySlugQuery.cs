using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobBySlug;


public record GetJobBySlugQuery(string Slug) : IRequest<Result<JobDetailsDto>>;