using AutoMapper;
using Kawadar.Application.Features.Auth.Dtos;

public class UserProfile : Profile
{
  public UserProfile()
  {
    CreateMap<string, UserDto>()
      .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src));

  }
}