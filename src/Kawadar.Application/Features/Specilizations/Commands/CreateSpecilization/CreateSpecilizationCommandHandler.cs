using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.CreateSpecilization
{
    public class CreateSpecilizationCommandHandler(IUnitOfWork unitOfWork, IUser user,
        ISpecilizationRepository specilizationRepository, IMapper mapper) : IRequestHandler<CreateSpecilizationCommand, Result<SpecilizationDTO>>
    {
        public async Task<Result<SpecilizationDTO>> Handle(CreateSpecilizationCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await specilizationRepository.GetByName(request.name);
            if (result.IsSuccess) return Error.Conflict("Specilization.alreadyExists", "A specilization with this name already exists");

            var specilization = Specilization.Create(request.name, request.isActive);
            var addResult = await specilizationRepository.AddAsync(specilization.Value);
            if (addResult.IsError) return addResult.Errors;

            var specilizationDTO = mapper.Map<SpecilizationDTO>(specilization.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return specilizationDTO;
        }
    }
}
