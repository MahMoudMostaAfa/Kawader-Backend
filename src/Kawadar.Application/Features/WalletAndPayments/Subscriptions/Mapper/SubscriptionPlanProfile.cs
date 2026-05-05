using AutoMapper;
using Kawadar.Application.Features.WalletAndPayments.Subscriptions.Dtos;
using Kawadar.Domain.Subscriptions;

namespace Kawadar.Application.Features.WalletAndPayments.Subscriptions.Mapper
{
    public class SubscriptionPlanProfile : Profile
    {
        public SubscriptionPlanProfile()
        {
            CreateMap<SubscriptionPlan, SubscriptionPlanDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.billingCycle, opt => opt.MapFrom(src => src.BillingCycleType))
                .ForMember(dest => dest.plan, opt => opt.MapFrom(src => src.Features));

            CreateMap<UserSubscription , UserSubscriptionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt))
                .ForMember(dest => dest.AutoRenew, opt => opt.MapFrom(src => src.AutoRenew));
        }
    }
}
