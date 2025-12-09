using Kawadar.Application.Features.Portfolios.DTOs;
using Kawadar.Domain.Portfolios.Project;

namespace Kawadar.Application.Features.Portfolios.Mapper
{
    public static class ProjectMapper
    {
        public static ProjectDTO toDTO(this PortfolioProject project)
        {
            var DTO = new ProjectDTO
            {
                title = project.Title,
                displayOrder = project.DisplayOrder,
                category = project.Category,
                description = project.Description
            };

            return DTO;
        }

        public static List<ProjectDTO> toDTOList(this IEnumerable<PortfolioProject> projectList)
        {
            return projectList.Select(p => p.toDTO()).ToList();
        }
    }
}
