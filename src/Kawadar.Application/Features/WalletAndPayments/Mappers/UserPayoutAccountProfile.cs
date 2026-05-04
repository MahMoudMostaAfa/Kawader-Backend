using AutoMapper;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.WalletAndPayments.Payouts;

namespace Kawadar.Application.Features.WalletAndPayments.Mappers;

public class UserPayoutAccountProfile : Profile
{
  public UserPayoutAccountProfile()
  {
    CreateMap<UserPayoutAccount, UserPayoutAccountDto>()
      .ForMember(dest => dest.AccountDetails, opt => opt.MapFrom(src => src.GetDetails().Value))
      .ForMember(dest => dest.PayoutType, opt => opt.MapFrom(src => src.PayoutType))
      .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DispalyName))
      .ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
      .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
      .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
  }
}
