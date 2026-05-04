using AutoMapper;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.WalletAndPayments.Payouts;

namespace Kawadar.Application.Features.WalletAndPayments.Mappers;

public class WithdrawalRequestProfile : Profile
{
  public WithdrawalRequestProfile()
  {
    CreateMap<WithdrawalRequest, WithdrawalRequestDto>()
      .ForMember(dest => dest.PayoutAccountId, opt => opt.MapFrom(src => src.UserPayoutAccountId))
      .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.WalletId))
      .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
      .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
      .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
      .ForMember(dest => dest.FailureReason, opt => opt.MapFrom(src => src.FailureReason))
      .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
      .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
      .ForMember(dest => dest.ProcessedAt, opt => opt.MapFrom(src => src.ProcessedAt))
      .ForMember(dest => dest.WalletTransactionId, opt => opt.MapFrom(src => src.WalletTransactionId))
      .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
  }
}
