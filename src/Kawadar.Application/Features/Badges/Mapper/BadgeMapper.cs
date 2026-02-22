using AutoMapper;
using Kawadar.Application.Features.Badges.DTOs;
using Kawadar.Domain.Badges;

namespace Kawadar.Application.Features.Badges.Mapper
{
    public class BadgeMapper: Profile
    {
        public BadgeMapper()
        {
            CreateMap<Badge, BadgeDTO>()

                .ForMember(dest => dest.Id, op => op.MapFrom(src => src.Id))

                .ForMember(dest => dest.Title, op => op.MapFrom(src => src.Title))

                .ForMember(dest => dest.IconUrl, op => op.MapFrom(src => src.IconUrl))

                .ForMember(dest => dest.Description, op => op.MapFrom(src => src.Description));
        }
    }
}
