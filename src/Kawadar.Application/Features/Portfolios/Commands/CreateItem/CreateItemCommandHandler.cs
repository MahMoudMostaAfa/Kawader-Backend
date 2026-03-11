using AutoMapper;
using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Common.Results.Abstractions;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Items.Enum;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateItem
{
    public class CreateItemCommandHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IMapper mapper, IStorageClient storageClient) : IRequestHandler<CreateItemCommand, Result<ItemDTO>>
    {
        public async Task<Result<ItemDTO>> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var items = await projectRepository.GetProjectItemsByProjectId(request.PortfolioProjectId);
            var displayOrder = items.Count() + 1;

            PortfolioItem? Item = null;
            if (request.ItemType == ItemType.Image && request.file is not null)
            {
                var Image = request.file;
                using var stream = Image.OpenReadStream();
                var uploadResult = await storageClient.UploadFileAsync(stream, Image.FileName, Containers.PortfolioProjectItems, cancellationToken);
                if (uploadResult.IsError) return uploadResult.Errors;
                var content = uploadResult.Value;

                var result = PortfolioItem.Create(request.ItemType, content,
                        displayOrder, request.PortfolioProjectId);
                if (result.IsError) return result.Errors;
                Item = result.Value;
            }

            else if(request.ItemType != ItemType.Image && request.Content is not null)
            {
                var result = PortfolioItem.Create(request.ItemType, request.Content, displayOrder,
                       request.PortfolioProjectId);
                if (result.IsError) return result.Errors;
                Item = result.Value;
            }
            else
            {
                return Error.Validation("Items can only be Image, Text or Link.");
            }
            
            var addResult = await projectRepository.AddItemAsync(Item);

            if (addResult.IsError) return addResult.Errors;

            var itemDTO = mapper.Map<ItemDTO>(Item);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return itemDTO;

        }
    }
}
