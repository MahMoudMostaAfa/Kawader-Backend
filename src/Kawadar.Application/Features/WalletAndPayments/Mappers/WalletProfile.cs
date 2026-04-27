using AutoMapper;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.WalletAndPayments;

namespace Kawadar.Application.Features.WalletAndPayments.Mappers;

public class WalletProfile : Profile
{
  public WalletProfile()
  {
    CreateMap<Wallet, WalletDto>()
      .ForMember(dest => dest.TotalBalance, opt => opt.MapFrom(src => src.TotalBalance))
      .ForMember(dest => dest.EscrowBalance, opt => opt.MapFrom(src => src.EscrowBalance))
      .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
      .ForMember(des => des.Id, opt => opt.MapFrom(src => src.Id))
      ;
  }
}