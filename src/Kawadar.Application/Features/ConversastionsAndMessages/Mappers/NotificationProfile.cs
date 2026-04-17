using AutoMapper;
using Kawadar.Application.Features.ConversastionsAndMessages.DTOs;
using Kawadar.Domain.Notifications;

namespace Kawadar.Application.Features.ConversastionsAndMessages.Mappers;


public class NotificationProfile : Profile
{
  public NotificationProfile()
  {
    CreateMap<Notification, NotificationDto>()
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
    .ForMember(dest => dest.Body, opt => opt.MapFrom(src => src.Body))
    .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.ToString()))
    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
    .ForMember(dest => dest.RedirectUrl, opt => opt.MapFrom(src => src.RedirectUrl))
    .ForMember(dest => dest.IsRead, opt => opt.MapFrom(src => src.IsRead))
    .ForMember(dest => dest.ReceivedAt, opt => opt.MapFrom(src => src.CreatedAt))
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
  }
}