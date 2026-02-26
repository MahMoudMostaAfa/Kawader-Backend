using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Skills;
using MediatR;

namespace Kawadar.Application.Features.Skills.Queries.GetSkillById
{
    public class GetSkillByIdHandler(IUser user, ISkillRepository skillRepository) : IRequestHandler<GetSkillByIdQuery, Result<Skill>>
    {
        public async Task<Result<Skill>> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var skillResult = await skillRepository.getByIdAsync(request.Id);
            if (skillResult.IsError) return skillResult.Errors;

            return skillResult.Value;
        }
    }
}
