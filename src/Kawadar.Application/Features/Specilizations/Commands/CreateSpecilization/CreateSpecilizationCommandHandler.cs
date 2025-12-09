using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using MediatR;
using System.Reflection.Metadata.Ecma335;

namespace Kawadar.Application.Features.Specilizations.Commands.CreateSpecilization
{
    public class CreateSpecilizationCommandHandler(IUnitOfWork unitOfWork, IUser user, ISpecilizationRepository specilizationRepository) : IRequestHandler<CreateSpecilizationCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(CreateSpecilizationCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await specilizationRepository.GetByName(request.name);
            if (!(result.Value is null)) return Error.Conflict("Specilization.AlreadyExists", "A specilization with this name already exists");

            var specilization = Specilization.Create(request.name, request.isActive);
            var addResult = await specilizationRepository.AddAsync(specilization.Value);
            if (addResult.IsError) return addResult.Errors;
            return addResult;
        }
    }
}
