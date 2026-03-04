using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Admins.Dtos;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Application.Features.Admins.Mapper
{
    public class AdminMapper : Profile
    {
        public AdminMapper()
        {
            CreateMap<(UserProfile userProfile, UserDto user), AdminDto>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.userProfile.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.userProfile.LastName))
                .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.userProfile.IsOnline))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.user.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user.Email))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.userProfile.IsDeleted));
        }
    }
}
