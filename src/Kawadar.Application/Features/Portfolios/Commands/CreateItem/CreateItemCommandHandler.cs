using AutoMapper;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateItem
{
    public class CreateItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IMapper mapper) : IRequestHandler<CreateItemCommand, Result<ItemDTO>>
    {
        public async Task<Result<ItemDTO>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var items = await projectRepository.GetProjectItemsByProjectId(request.PortfolioProjectId);
            var displayOrder = items.Count() + 1;

            var result = PortfolioItem.Create(request.ItemType, request.Content, displayOrder,
                   request.PortfolioProjectId);
            
            if (result.IsError) return result.Errors;

            var Item = result.Value;
            var addResult = await projectRepository.AddItemAsync(Item);

            if (addResult.IsError) return addResult.Errors;

            var itemDTO = mapper.Map<ItemDTO>(Item);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return itemDTO;

        }
    }
}
