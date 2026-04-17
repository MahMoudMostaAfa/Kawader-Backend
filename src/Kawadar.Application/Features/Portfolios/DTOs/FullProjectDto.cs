
namespace Kawadar.Application.Features.Portfolios.DTOs
{
    public class FullProjectDto
    {
        public Guid Id { get; set; }
        public string title { get; set; } = "";
        public string description { get; set; } = "";
        public string ProjectImageUrl { get; set; } = "";
        public string ProjectUrl { get; set; } = "";
        public int displayOrder { get; set; }
        public List<ItemDTO> Items { get; set; } = new List<ItemDTO>();
        public List<string> Skills { get; set; } = new List<string>();
    }
}
