using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Application.Features.Specilizations.Mapper;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Queries.GetSpecilizationById
{
    public class GetSpecilizationByIdQueryHandler(IUnitOfWork unitOfWork, IUser user, ISpecilizationRepository specilizationRepository)
        : IRequestHandler<GetSpecilizationByIdQuery, Result<SpecilizationDTO>>
    {
        public async Task<Result<SpecilizationDTO>> Handle(GetSpecilizationByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await specilizationRepository.GetById(request.Id);
            if (result.IsError) return result.Errors;

            var specilization = result.Value;
            return specilization.toDTO();
        }
    }
}
