using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Application.Features.ProfileManagment.Mappers;

class UserProfileToUserProfileDtoProfile : Profile
{
  public UserProfileToUserProfileDtoProfile()
  {

    CreateMap<(UserProfile userProfile, UserDto user), UserProfileDto>()
    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.userProfile.FirstName))
    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.userProfile.LastName))
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.userProfile.Title))
    .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.userProfile.Bio))
    .ForMember(dest => dest.ExperienceYear, opt => opt.MapFrom(src => src.userProfile.ExperienceYear))
    .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom(src => src.userProfile.ProfilePictureUrl))
    .ForMember(dest => dest.VideoLink, opt => opt.MapFrom(src => src.userProfile.VideoLink))
    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.userProfile.PhoneNumber))
    .ForMember(dest => dest.IsAvailable, opt => opt.MapFrom(src => src.userProfile.IsAvailable))

    .ForMember(dest => dest.IsActivated, opt => opt.MapFrom(src => src.userProfile.IsActivated))
    .ForMember(dest => dest.ActivatedAt, opt => opt.MapFrom(src => src.userProfile.ActivatedAt))
    .ForMember(dest => dest.IsOnline, opt => opt.MapFrom(src => src.userProfile.IsOnline))
    .ForMember(dest => dest.IsIdentityVerified, opt => opt.MapFrom(src => src.userProfile.IsIdentityVerified))
    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.user.UserName))
    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.user.Email))
    .ForMember(dest => dest.ProfileType, opt => opt.MapFrom(src => src.userProfile.ProfileType))
    .ForMember(dest => dest.specilizationId, opt => opt.MapFrom(src => src.userProfile.SpecializationId))
    .ForMember(dest => dest.IsBanned, opt => opt.MapFrom(src => src.userProfile.IsBanned))
    .ForMember(dest => dest.BannedUntil, opt => opt.MapFrom(src => src.userProfile.BannedUntil))
    .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.userProfile.IsDeleted));

  }
}