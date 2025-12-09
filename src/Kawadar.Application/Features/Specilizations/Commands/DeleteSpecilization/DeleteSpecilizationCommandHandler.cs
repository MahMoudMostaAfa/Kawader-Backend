using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Specilizations;
using MediatR;

namespace Kawadar.Application.Features.Specilizations.Commands.DeleteSpecilization
{
    public class DeleteSpecilizationCommandHandler(IUnitOfWork unitOfWork, IUser user, ISpecilizationRepository specilizationRepository) : IRequestHandler<DeleteSpecilizationCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteSpecilizationCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = await specilizationRepository.GetById(request.Id);
            if (result.IsError) return result.Errors;

            var deleteResult = specilizationRepository.Delete(result.Value);
            if (deleteResult.IsError) return deleteResult.Errors;
            return deleteResult;
        }
    }
}
