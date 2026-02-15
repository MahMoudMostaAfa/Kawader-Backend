using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Queries.GetSpecilizationById
{
    public record GetSpecilizationByIdQuery(Guid Id) : IRequest<Result<SpecilizationDTO>>;
}
