using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Application.Features.Specilizations.Mapper;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Queries.GetAllSpecilizations
{
    public class GetAllSpecilizationsQueryHandler(IUser user, ISpecilizationRepository specilizationRepository) : IRequestHandler<GetAllSpecilizationsQuery, Result<List<SpecilizationDTO>>>
    {
        public async Task<Result<List<SpecilizationDTO>>> Handle(GetAllSpecilizationsQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var specilizations = await specilizationRepository.GetAll(cancellationToken);

            List<SpecilizationDTO> specilizationDTOs = specilizations.toDTOList();
            return specilizationDTOs;
        }
    }
}