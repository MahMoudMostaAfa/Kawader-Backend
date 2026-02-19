using AutoMapper;
using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Specilizations;

namespace Kawadar.Application.Features.Specilizations.Mapper
{
    public class SpecilizationMapper : Profile
    {
        public SpecilizationMapper()
        {
            CreateMap<Specilization, SpecilizationDTO>()

                .ForMember(dest => dest.Id, op => op.MapFrom(src => src.Id))

                .ForMember(dest => dest.Name, op => op.MapFrom(src => src.Name))

                .ForMember(dest => dest.IsActive, op => op.MapFrom(src => src.IsActive));
        }
    }
}
