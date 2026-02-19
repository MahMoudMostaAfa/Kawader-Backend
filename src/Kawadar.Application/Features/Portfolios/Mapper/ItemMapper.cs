using AutoMapper;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Portfolios.Items;

namespace Kawadar.Application.Features.Portfolios.Mapper
{
    public class ItemMapper : Profile
    {
        public ItemMapper()
        {
            CreateMap<PortfolioItem, ItemDTO>()

                .ForMember(dest => dest.Id, op => op.MapFrom(src => src.Id))

                .ForMember(dest => dest.itemType, op => op.MapFrom(src => src.ItemType))

                .ForMember(dest => dest.content, op => op.MapFrom(src => src.Content))

                .ForMember(dest => dest.displayOrder, op => op.MapFrom(src => src.DisplayOrder));
        }
    }
}
