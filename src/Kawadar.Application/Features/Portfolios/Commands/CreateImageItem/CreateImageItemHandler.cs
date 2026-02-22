
using AutoMapper;
using Kawadar.Application.Common.Constants;
using Kawadar.Application.Common.Errors;
using Kawadar.Application.Common.Interfaces.Auth;
using Kawadar.Application.Common.Interfaces.Repositories;
using Kawadar.Application.Common.Interfaces;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Application.Features.Portfolios.Mapper;
using Kawadar.Domain.Common.Results;
using Kawadar.Domain.Portfolios.Items;
using Kawadar.Domain.Portfolios.Project;
using MediatR;

namespace Kawadar.Application.Features.Portfolios.Commands.CreateImageItem
{
    public class CreateImageItemHandler(IUser user, IUnitOfWork unitOfWork,
        IPortfolioProjectRepository projectRepository, IStorageClient storageClient, IMapper mapper) : IRequestHandler<CreateImageItemCommand, Result<ItemDTO>>
    {
        public async Task<Result<ItemDTO>> Handle(CreateImageItemCommand request, CancellationToken cancellationToken)
        {
            var userId = user.Id;
            if (userId is null) return ApplicationErrors.UserIsNotAuthenticated;

            var Items = await projectRepository.GetProjectItemsByProjectId(request.PortfolioProjectId);
            var displayOrder = Items.Count() + 1;

            var Image = request.Image;
            using var stream = Image.OpenReadStream();
            var uploadResult = await storageClient.UploadFileAsync(stream, Image.FileName, Containers.PortfolioProjectItems, cancellationToken);
            if (uploadResult.IsError) return uploadResult.Errors;
            var content = uploadResult.Value;

            var result = PortfolioItem.Create(request.ItemType, content,
                    displayOrder, request.PortfolioProjectId);

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
