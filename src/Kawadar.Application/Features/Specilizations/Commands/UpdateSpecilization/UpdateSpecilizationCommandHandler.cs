using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.UpdateSpecilization
{
    public class UpdateSpecilizationCommandHandler(IUnitOfWork unitOfWork, IUser user, ISpecilizationRepository specilizationRepository) : IRequestHandler<UpdateSpecilizationCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(UpdateSpecilizationCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await specilizationRepository.GetById(request.Id);
            if (result.IsError) return result.Errors;

            var specilization = result.Value;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return specilization.Update(request.name, request.isActive);
        }
    }
}
