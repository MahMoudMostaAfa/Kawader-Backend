using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Queries.GetSpecilizationById
{
    public class GetSpecilizationByIdQueryHandler(IUser user, ISpecilizationRepository specilizationRepository
        , IMapper mapper): IRequestHandler<GetSpecilizationByIdQuery, Result<SpecilizationDTO>>
    {
        public async Task<Result<SpecilizationDTO>> Handle(GetSpecilizationByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await specilizationRepository.GetById(request.Id);
            if (result.IsError) return result.Errors;

            var specilization = result.Value;

            var specilizationDTO = mapper.Map<SpecilizationDTO>(specilization);
            return specilizationDTO;
        }
    }
}
