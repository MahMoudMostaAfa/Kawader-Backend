using Kawadar.Application.Features.Specilizations.DTO;
using Kawadar.Domain.Specilizations;

namespace Kawadar.Application.Features.Specilizations.Mapper
{
    public static class SpecilizationMapper
    {
        public static SpecilizationDTO toDTO(this Specilization specilization)
        {
            var specilizationDTO = new SpecilizationDTO { Id = specilization.Id, Name = specilization.Name, IsActive = specilization.IsActive };
            return specilizationDTO;
        }

        public static List<SpecilizationDTO> toDTOList(this IEnumerable<Specilization> specilizations)
        {
            return specilizations.Select(s => s.toDTO()).ToList();
        }
    }
}
