using AutoMapper;
using Kawadar.Application.Common.Models;
using Kawadar.Application.Features.Contracts.Disbutes.Dtos;
using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Contracts;
using Kawadar.Domain.UserProfiles;


namespace Kawadar.Application.Features.Contracts.Mappers;

public class ContractDetailsDtoProfile : Profile
{
  public ContractDetailsDtoProfile()
  {
    CreateMap<(Contract c, UserDto otherParty, UserProfile otherPartyProfile), ContractDetailsDto>()
    .ForMember(des => des.Id, opt => opt.MapFrom(src => src.c.Id))
    .ForMember(des => des.JobId, opt => opt.MapFrom(src => src.c.JobId))
    .ForMember(des => des.ProposalId, opt => opt.MapFrom(src => src.c.ProposalId))
    .ForMember(des => des.OtherPartyId, opt => opt.MapFrom(src => src.otherPartyProfile.Id))
    .ForMember(des => des.OtherPartyName, opt => opt.MapFrom(src => src.otherPartyProfile.FullName))
    .ForMember(des => des.OtherPartyProfilePictureUrl, opt => opt.MapFrom(src => src.otherPartyProfile.ProfilePictureUrl))
    .ForMember(des => des.OtherPartyUsername, opt => opt.MapFrom(src => src.otherParty.UserName))
    .ForMember(des => des.ContractType, opt => opt.MapFrom(src => src.c.Type))
    .ForMember(des => des.OneTimeFixedPrice, opt => opt.MapFrom(src => src.c.OneTimeFixedPrice))
    .ForMember(des => des.StartDate, opt => opt.MapFrom(src => src.c.StartAt))
    .ForMember(des => des.EndDate, opt => opt.MapFrom(src => src.c.EndAt))
    .ForMember(des => des.Status, opt => opt.MapFrom(src => src.c.Status))
    .ForMember(des => des.Role, opt => opt.MapFrom(src => src.c.ClientId == src.otherPartyProfile.Id ? ContractRole.Freelancer : ContractRole.Client))
    .ForMember(des => des.Title, opt => opt.MapFrom(src => src.c.Title))
    .ForMember(des => des.Description, opt => opt.MapFrom(src => src.c.Description))
    .ForMember(des => des.TotalMilestones, opt => opt.MapFrom(src => src.c.ContractMilestones.Count))
    .ForMember(des => des.Milestones, opt => opt.MapFrom(src => src.c.ContractMilestones.Select(cm => new ContractMilestoneDto
    {
      Id = cm.Id,
      ProposalMilestoneId = cm.ProposalMilestoneId,
      Title = cm.Title,
      Description = cm.Description,
      Amount = cm.Amount,
      DueDate = cm.DueDate,
      CompletionRequestedAt = cm.CompletionRequestedAt,
      CompletionApprovedAt = cm.CompletionApprovedAt,
      RejectionReason = cm.RejectionReason,
      Order = cm.Order,
      Status = cm.Status
    }).ToList()));

        CreateMap<(Contract c, UserDto freelancer, UserDto client), AdminContractDto>()
            .ForMember(des => des.Id, opt => opt.MapFrom(src => src.c.Id))
            .ForMember(des => des.JobId, opt => opt.MapFrom(src => src.c.JobId))
            .ForMember(des => des.ProposalId, opt => opt.MapFrom(src => src.c.ProposalId))
            .ForMember(des => des.FreelancerId, opt => opt.MapFrom(src => src.c.FreelancerId))
            .ForMember(des => des.ClientId, opt => opt.MapFrom(src => src.c.ClientId))
            .ForMember(des => des.ClientUserName, opt => opt.MapFrom(src => src.client.UserName))
            .ForMember(des => des.FreelancerUsername, opt => opt.MapFrom(src => src.freelancer.UserName))
            .ForMember(des => des.ContractType, opt => opt.MapFrom(src => src.c.Type))
            .ForMember(des => des.OneTimeFixedPrice, opt => opt.MapFrom(src => src.c.OneTimeFixedPrice))
            .ForMember(des => des.StartDate, opt => opt.MapFrom(src => src.c.StartAt))
            .ForMember(des => des.EndDate, opt => opt.MapFrom(src => src.c.EndAt))
            .ForMember(des => des.Status, opt => opt.MapFrom(src => src.c.Status))
            .ForMember(des => des.Title, opt => opt.MapFrom(src => src.c.Title))
            .ForMember(des => des.Description, opt => opt.MapFrom(src => src.c.Description))
            .ForMember(des => des.TotalMilestones, opt => opt.MapFrom(src => src.c.ContractMilestones.Count))
            .ForMember(des => des.Milestones, opt => opt.MapFrom(src => src.c.ContractMilestones.Select(cm => new ContractMilestoneDto
            {
                Id = cm.Id,
                ProposalMilestoneId = cm.ProposalMilestoneId,
                Title = cm.Title,
                Description = cm.Description,
                Amount = cm.Amount,
                DueDate = cm.DueDate,
                CompletionRequestedAt = cm.CompletionRequestedAt,
                CompletionApprovedAt = cm.CompletionApprovedAt,
                RejectionReason = cm.RejectionReason,
                Order = cm.Order,
                Status = cm.Status
            }).ToList()));
    }
}