using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Jobs.Queries.GetJobReport
{
    public record GetJobReportQuery(Guid Id) : IRequest<Result<FullJobReportDto>>, ICachedQuery
    {
        public string CacheKey => $"GetJobReportQuery-{Id}";

        public string[] Tags => ["JobReports"];
    }
}
