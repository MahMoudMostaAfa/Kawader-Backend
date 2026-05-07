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

    CreateMap<Wallet, AdminWalletDto>()
      .ForMember(dest => dest.TotalBalance, opt => opt.MapFrom(src => src.TotalBalance))
      .ForMember(dest => dest.EscrowBalance, opt => opt.MapFrom(src => src.EscrowBalance))
      .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
      .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
      .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
      .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));
  }
}