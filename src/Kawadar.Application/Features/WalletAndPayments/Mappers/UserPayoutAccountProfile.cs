using AutoMapper;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.WalletAndPayments.Payouts;

namespace Kawadar.Application.Features.WalletAndPayments.Mappers;

public class UserPayoutAccountProfile : Profile
{
  public UserPayoutAccountProfile()
  {
    CreateMap<UserPayoutAccount, UserPayoutAccountDto>()
      .ForMember(dest => dest.AccountDetails, opt => opt.MapFrom(src => src.GetDetails().Value));
  }
}
