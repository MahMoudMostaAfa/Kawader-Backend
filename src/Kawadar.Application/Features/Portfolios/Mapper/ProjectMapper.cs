using AutoMapper;
using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Portfolios.Project;

namespace Kawadar.Application.Features.Portfolios.Mapper
{
    public class ProjectMapper: Profile
    {
        public ProjectMapper()
        {
            CreateMap<PortfolioProject, ProjectDTO>()

                .ForMember(dest => dest.Id, op => op.MapFrom(src => src.Id))

                .ForMember(dest => dest.ProjectImageUrl, op => op.MapFrom(src => src.ProjectImageUrl))

                .ForMember(dest => dest.ProjectUrl, op => op.MapFrom(src => src.ProjectUrl))

                .ForMember(dest => dest.title, op => op.MapFrom(src => src.Title))

                .ForMember(dest => dest.description, op => op.MapFrom(src => src.Description))
                
                .ForMember(dest => dest.displayOrder, op => op.MapFrom(src => src.DisplayOrder));
        }
    }
}
