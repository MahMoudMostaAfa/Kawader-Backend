
using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Badges
{
    public class Badge: AuditableEntity
    {
        public string Title { get; private set; }

        public string IconUrl { get; private set; }
        public string Description { get; private set; }


        private Badge(string Title, string IconUrl, string Description) : base(Guid.NewGuid())
        {
            this.Title = Title;
            this.IconUrl = IconUrl;
            this.Description = Description;
        }

        public static Result<Badge> Create(string Title, string IconUrl, string Description)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                return BadgeErrors.TitleIsEmpty;
            }

            if (string.IsNullOrWhiteSpace(IconUrl))
            {
                return BadgeErrors.IconIsEmpty;
            }

            if (string.IsNullOrWhiteSpace(Description))
            {
                return BadgeErrors.DescriptionIsEmpty;
            }

            var badge = new Badge(Title, IconUrl, Description);
            return badge;
        }

        public Result<Updated> Update(string IconUrl)
        {
            this.IconUrl = IconUrl;
            UpdatedAt = DateTime.UtcNow;

            return Result.Updated;
        }
    }
}
