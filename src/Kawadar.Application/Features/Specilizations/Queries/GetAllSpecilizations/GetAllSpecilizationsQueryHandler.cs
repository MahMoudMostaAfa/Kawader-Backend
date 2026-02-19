using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Queries.GetAllSpecilizations
{
    public class GetAllSpecilizationsQueryHandler(IUser user, ISpecilizationRepository specilizationRepository
        , IMapper mapper) : IRequestHandler<GetAllSpecilizationsQuery, Result<List<SpecilizationDTO>>>
    {
        public async Task<Result<List<SpecilizationDTO>>> Handle(GetAllSpecilizationsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var specilizations = await specilizationRepository.GetAll(cancellationToken);

            var specilizationDTOs = mapper.Map<List<SpecilizationDTO>>(specilizations);
            return specilizationDTOs;
        }
    }
}