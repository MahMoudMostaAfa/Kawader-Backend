using AutoMapper;
using Kawadar.Application.Features.WalletAndPayments.DTOs;
using Kawadar.Domain.WalletAndPayments;

namespace Kawadar.Application.Features.WalletAndPayments.Mappers
{
    public class WalletTransactionProfile : Profile
    {
        public WalletTransactionProfile()
        {
            CreateMap<WalletTransaction, TransactionDto>()
                .ForMember(dest => dest.WalletId, opt => opt.MapFrom(src => src.WalletId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ReferenceType, opt => opt.MapFrom(src => src.ReferenceType))
                .ForMember(dest => dest.ReferenceId, opt => opt.MapFrom(src => src.ReferenceId))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.BalanceBefore, opt => opt.MapFrom(src => src.BalanceBefore))
                .ForMember(dest => dest.BalanceAfter, opt => opt.MapFrom(src => src.BalanceAfter))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));
        }
    }
}
