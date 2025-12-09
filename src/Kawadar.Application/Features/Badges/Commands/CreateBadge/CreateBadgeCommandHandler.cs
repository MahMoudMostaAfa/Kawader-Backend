using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Badges;
using Kawadar.Domain.Common.Results;
using MediatR;


namespace Kawadar.Application.Features.Badges.Commands.CreateBadge
{
    public class CreateBadgeCommandHandler(IUnitOfWork unitOfWork,IUser user, IBadgeRepository badgeRepository) : IRequestHandler<CreateBadgeCommand, Result<Success>>
    {
        public async Task<Result<Success>> Handle(CreateBadgeCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var result = Badge.Create(request.title, request.IconUrl, request.description);

            if (result.IsError) return result.Errors;

            await badgeRepository.AddAsync(result.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success;
        }
    }
}
