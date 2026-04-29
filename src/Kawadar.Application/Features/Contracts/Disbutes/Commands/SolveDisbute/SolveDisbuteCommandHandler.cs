using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Contracts.Disbutes.Commands.SolveDisbute
{
    public class SolveDisbuteCommandHandler(IUser user, IDisbuteRepository disbuteRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<SolveDisbuteCommand, Result<Updated>>
    {
        public async Task<Result<Updated>> Handle(SolveDisbuteCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var disbuteResult = await disbuteRepository.GetDisbuteById(request.DisbuteId);
            if (disbuteResult.IsError) return disbuteResult.Errors;

            if(request.resolution is not null) 
            {
                disbuteResult.Value.Update(request.status, request.resolution, DateTime.UtcNow);
            }
            else
            {
                disbuteResult.Value.Update(request.status, null, null);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Updated;
        }
    }
}
