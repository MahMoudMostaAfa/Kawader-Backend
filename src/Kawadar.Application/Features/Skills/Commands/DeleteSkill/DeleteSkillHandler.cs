using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Skills.Commands.DeleteSkill
{
    public class DeleteSkillHandler(IUser user, ISkillRepository skillRepository
        , IUnitOfWork unitOfWork) : IRequestHandler<DeleteSkillCommand, Result<Deleted>>
    {
        public async Task<Result<Deleted>> Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var existsResult = await skillRepository.getByIdAsync(request.Id);
            if (existsResult.IsError) return Error.NotFound("Skill.NotFound", "There is no skill with the specified Id");

            var skill = existsResult.Value;
            var deleteResult = skillRepository.delete(skill);

            await unitOfWork.SaveChangesAsync();
            return Result.Deleted;
        }
    }
}
