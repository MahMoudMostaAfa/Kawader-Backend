using AutoMapper;
using Kawadar.Application.Features.Contracts.Dtos;
using Kawadar.Domain.Contracts;

namespace Kawadar.Application.Features.Contracts.Mappers;

public class ContractDtoProfile : Profile
{
  public ContractDtoProfile()
  {
    CreateMap<(Contract c, Guid cu), ContractDto>()
    .ForMember(des => des.Id, opt => opt.MapFrom(src => src.c.Id))
    .ForMember(des => des.JobId, opt => opt.MapFrom(src => src.c.JobId))
    .ForMember(des => des.ProposalId, opt => opt.MapFrom(src => src.c.ProposalId))
    .ForMember(des => des.OtherPartyId, opt => opt.MapFrom(src => src.c.ClientId == src.cu ? src.c.FreelancerId : src.c.ClientId))
    .ForMember(des => des.ContractType, opt => opt.MapFrom(src => src.c.Type))
    .ForMember(des => des.OneTimeFixedPrice, opt => opt.MapFrom(src => src.c.OneTimeFixedPrice))
    .ForMember(des => des.StartDate, opt => opt.MapFrom(src => src.c.StartAt))
    .ForMember(des => des.EndDate, opt => opt.MapFrom(src => src.c.EndAt))
    .ForMember(des => des.Status, opt => opt.MapFrom(src => src.c.Status))
    .ForMember(des => des.Role, opt => opt.MapFrom(src => src.c.ClientId == src.cu ? ContractRole.Client : ContractRole.Freelancer))
    .ForMember(des => des.Title, opt => opt.MapFrom(src => src.c.Title))
    .ForMember(des => des.Description, opt => opt.MapFrom(src => src.c.Description))
    .ForMember(des => des.TotalMilestones, opt => opt.MapFrom(src => src.c.ContractMilestones.Count));


  }
}