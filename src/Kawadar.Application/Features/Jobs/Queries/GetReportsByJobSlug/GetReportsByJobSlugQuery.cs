using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetReportsByJobSlug
{
    public record GetReportsByJobSlugQuery(string JobSlug) : IRequest<Result<List<BriefJobReportDto>>>;
}
