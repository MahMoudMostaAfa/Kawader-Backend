using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Jobs.DTOs;
using Kawadar.Domain.Jobs.JobFiles;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Skills;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Application.Features.Jobs.Mappers;

public class JobDetailsProfile : Profile
{
  public JobDetailsProfile()
  {
    CreateMap<(Kawadar.Domain.Jobs.Job job, UserDto userDto, UserProfile userProfile), JobDetailsDto>()
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.job.Title))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.job.Description))
    .ForMember(dest => dest.PosterFullName, opt => opt.MapFrom(src => src.userProfile.FullName))
    .ForMember(dest => dest.posterProfilePictureUrl, opt => opt.MapFrom(src => src.userProfile.ProfilePictureUrl))
    .ForMember(dest => dest.PosterUsername, opt => opt.MapFrom(src => src.userDto.UserName))
    .ForMember(dest => dest.JobSlug, opt => opt.MapFrom(src => Uri.EscapeDataString(src.job.JobSlug)))
    .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.job.Questions))
    .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.job.Skills))
    .ForMember(dest => dest.Attachments, opt => opt.MapFrom(src => src.job.Attachments))
    .ForMember(dest => dest.Specilization, opt => opt.MapFrom(src => src.job.Specilization.Name))
    .ForMember(dest => dest.JobType, opt => opt.MapFrom(src => src.job.JobType))
    .ForMember(dest => dest.BudgetRange, opt => opt.MapFrom(src => src.job.BudgetRange))
    .ForMember(dest => dest.HourlyRateRange, opt => opt.MapFrom(src => src.job.HourlyRateRange))
    .ForMember(dest => dest.DurationInDays, opt => opt.MapFrom(src => src.job.DurationInDays))
    .ForMember(dest => dest.ExperienceLevel, opt => opt.MapFrom(src => src.job.ExperienceLevel))
    .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => src.job.JobStatus));

    CreateMap<JobFile, JobAttachmentDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File.FileName))
    .ForMember(dest => dest.FileUrl, opt => opt.MapFrom(src => src.File.FileUrl))
    .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.File.MimeType))
    .ForMember(dest => dest.FileSizeInBytes, opt => opt.MapFrom(src => src.File.FileSizeInBytes));

    CreateMap<JobQuestion, JobQuestionDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.QuestionText, opt => opt.MapFrom(src => src.Question))
    .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => src.IsRequired))
    .ForMember(dest => dest.DisplayOrder, opt => opt.MapFrom(src => src.DisplayOrder));

    CreateMap<Skill, JobSkillDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Name));

    CreateMap<Kawadar.Domain.Jobs.Job, JobSummaryDto>()
    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
    .ForMember(dest => dest.JobSlug, opt => opt.MapFrom(src => Uri.EscapeDataString(src.JobSlug)))
    .ForMember(dest => dest.Specilization, opt => opt.MapFrom(src => src.Specilization.Name))
    .ForMember(dest => dest.JobType, opt => opt.MapFrom(src => src.JobType))
    .ForMember(dest => dest.BudgetRange, opt => opt.MapFrom(src => src.BudgetRange))
    .ForMember(dest => dest.HourlyRateRange, opt => opt.MapFrom(src => src.HourlyRateRange))
    .ForMember(dest => dest.DurationInDays, opt => opt.MapFrom(src => src.DurationInDays))
    .ForMember(dest => dest.ExperienceLevel, opt => opt.MapFrom(src => src.ExperienceLevel))
    .ForMember(dest => dest.JobStatus, opt => opt.MapFrom(src => src.JobStatus))
    .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
    .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
  }
}