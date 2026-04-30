using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Violations.Dtos;
using Kawadar.Domain.Violations;

namespace Kawadar.Application.Features.Violations.Mapper
{
    public class ViolationProfile : Profile
    {
        public ViolationProfile()
        {
            CreateMap<(Violation violation, UserDto userDto), BriefViolationDto>()
                .ForMember(dest => dest.userName, opt => opt.MapFrom(src => src.userDto.UserName))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.violation.Id))
                .ForMember(dest => dest.violationStatus, opt => opt.MapFrom(src => src.violation.ViolationStatus))
                .ForMember(dest => dest.violationType, opt => opt.MapFrom(src => src.violation.ViolationType))
                .ForMember(dest => dest.severityScore, opt => opt.MapFrom(src => src.violation.severityScore))
                .ForMember(dest => dest.RefernceType, opt => opt.MapFrom(src => src.violation.ReferenceType));

            CreateMap<(Violation violation, UserDto user), FullViolationDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.user.UserName))
                .ForMember(dest => dest.ViolationStatus, opt => opt.MapFrom(src => src.violation.ViolationStatus))
                .ForMember(dest => dest.ViolationType, opt => opt.MapFrom(src => src.violation.ViolationType))
                .ForMember(dest => dest.severityScore, opt => opt.MapFrom(src => src.violation.severityScore))
                .ForMember(dest => dest.ReferenceType, opt => opt.MapFrom(src => src.violation.ReferenceType))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.violation.Id))
                .ForMember(dest => dest.ViolationEvidence, opt => opt.MapFrom(src => src.violation.ViolationEvidence))
                .ForMember(dest => dest.ResolvedAt, opt => opt.MapFrom(src => src.violation.ResolvedAt))
                .ForMember(dest => dest.RedirectUrl, opt => opt.MapFrom(src => src.violation.RedirectUrl))
                .ForMember(dest => dest.NoteByAdmin, opt => opt.MapFrom(src => src.violation.NoteByAdmin))
                .ForMember(dest => dest.ActionTaken, opt => opt.MapFrom(src => src.violation.ActionTaken));
        }
    }
}
