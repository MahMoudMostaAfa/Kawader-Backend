using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Queries.GetAllSpecilizations
{
    public record GetAllSpecilizationsQuery: IRequest<Result<List<SpecilizationDTO>>>;
}
