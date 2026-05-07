using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Proposals.Dtos;
using Kawadar.Domain.Jobs.JobQuestions;
using Kawadar.Domain.Proposals;
using Kawadar.Domain.Proposals.ProposalMilestones;
using Kawadar.Domain.Proposals.QuestionAnswers;
using Kawadar.Domain.UserProfiles;

namespace Kawadar.Application.Features.Proposals.Mappers;


public class ProposalDetailsProfile : Profile
{
  public ProposalDetailsProfile()
  {
    CreateMap<(JobProposal jp, UserDto u, UserProfile up), ProposalDetailsDto>()
    .ForMember(dest => dest.CoverLetter, opt => opt.MapFrom(src => src.jp.CoverLetter))
    .ForMember(dest => dest.JobId, opt => opt.MapFrom(src => src.jp.JobId))
    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.jp.Amount))
    .ForMember(dest => dest.EstimatedDays, opt => opt.MapFrom(src => src.jp.EstimatedDays))
    .ForMember(dest => dest.EstimatedHours, opt => opt.MapFrom(src => src.jp.EstimatedHours))
    .ForMember(dest => dest.HourlyRate, opt => opt.MapFrom(src => src.jp.HourlyRate))
    .ForMember(dest => dest.EstimatedHours, opt => opt.MapFrom(src => src.jp.EstimatedHours))
    .ForMember(dest => dest.JobProposalType, opt => opt.MapFrom(src => src.jp.ProposalType))
    .ForMember(dest => dest.JobProposalType, opt => opt.MapFrom(src => src.jp.ProposalType))
    .ForMember(dest => dest.ProposalByFullName, opt => opt.MapFrom(src => src.up.FullName))
    .ForMember(dest => dest.ProposalByPhoto, opt => opt.MapFrom(src => src.up.ProfilePictureUrl))
    .ForMember(dest => dest.ProposalByUserName, opt => opt.MapFrom(src => src.u.UserName))
    .ForMember(dest => dest.Milestones, opt => opt.MapFrom(src => src.jp.Milestones))
    .ForMember(dest => dest.QuestionsWithAnswer, opt => opt.MapFrom(src => src.jp.QuestionAnswers))
      .ForMember(dest => dest.SubmittedAt, opt => opt.MapFrom(src => src.jp.CreatedAt))
    ;

    CreateMap<ProposalMilestone, MilestoneDto>().
    ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount)).
    ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description)).
    ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate)).
    ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title));

    CreateMap<ProposalQuestionAnswer, QuestionWithAnswerDto>().
    ForMember(dest => dest.Answer, opt => opt.MapFrom(src => src.Answer)).
    ForMember(dest => dest.Question, opt => opt.MapFrom(src => src.Question.Question));

  }
}