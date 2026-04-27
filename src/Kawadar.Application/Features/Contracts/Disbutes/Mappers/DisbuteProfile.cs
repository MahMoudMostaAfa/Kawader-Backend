using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Domain.Contracts.Disbutes;

namespace Kawadar.Application.Features.Contracts.Disbutes.Mappers
{
    public class DisbuteProfile : Profile
    {
        public DisbuteProfile()
        {
            CreateMap<(Disbute disbute, UserDto userDto), fullDisbuteDto>()
                .ForMember(dest => dest.contractId, opt => opt.MapFrom(src => src.disbute.ContractId))
                .ForMember(dest => dest.reason, opt => opt.MapFrom(src => src.disbute.Reason))
                .ForMember(dest => dest.resolution, opt => opt.MapFrom(src => src.disbute.Resolution))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.disbute.Status))
                .ForMember(dest => dest.ResolvedAt, opt => opt.MapFrom(src => src.disbute.ResolvedAt))
                .ForMember(dest => dest.RaisedByUserName, opt => opt.MapFrom(src => src.userDto.UserName));

            CreateMap<(Disbute disbute, UserDto userDto), BriefDisbuteDto>()
                .ForMember(dest => dest.reason, opt => opt.MapFrom(src => src.disbute.Reason))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.disbute.Status))
                .ForMember(dest => dest.RaisedByUserName, opt => opt.MapFrom(src => src.userDto.UserName));
        }
    }
}
