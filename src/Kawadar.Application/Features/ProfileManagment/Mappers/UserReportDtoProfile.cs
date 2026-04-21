using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.ProfileManagment.DTOs;
using Kawadar.Domain.UserProfiles.UserReports;

namespace Kawadar.Application.Features.ProfileManagment.Mappers
{
    public class UserReportDtoProfile : Profile
    {
        public UserReportDtoProfile()
        {
            CreateMap<(UserReport userReport, UserDto reporter, UserDto reported), BriefUserReportDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.userReport.Id))
                .ForMember(dest => dest.reportStatus, opt => opt.MapFrom(src => src.userReport.ReportStatus))
                .ForMember(dest => dest.reportType, opt => opt.MapFrom(src => src.userReport.ReportType))
                .ForMember(dest => dest.ReportedUserName, opt => opt.MapFrom(src => src.reported.UserName))
                .ForMember(dest => dest.ReporterUserName, opt => opt.MapFrom(src => src.reporter.UserName));

            CreateMap<(UserReport userReport, UserDto reporter, UserDto reported), FullUserReportDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.userReport.Id))
                .ForMember(dest => dest.reportStatus, opt => opt.MapFrom(src => src.userReport.ReportStatus))
                .ForMember(dest => dest.reportType, opt => opt.MapFrom(src => src.userReport.ReportType))
                .ForMember(dest => dest.ReportedUserName, opt => opt.MapFrom(src => src.reported.UserName))
                .ForMember(dest => dest.ReporterUserName, opt => opt.MapFrom(src => src.reporter.UserName))
                .ForMember(dest => dest.ActionTaken, opt => opt.MapFrom(src => src.userReport.ActionTaken))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.userReport.Content));


        }
    }
}
