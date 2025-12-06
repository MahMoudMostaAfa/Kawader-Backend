
using Kawadar.Domain.Common;
using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Badges
{
    public class Badge: AuditableEntity
    {
        public string title { get; private set; }

        public string iconUrl { get; private set; }
        public string description { get; private set; }


        private Badge(string Title, string IconUrl, string Description) : base(Guid.NewGuid())
        {
            title = Title;
            iconUrl = IconUrl;
            description = Description;
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

            var badge = new Badge(Title, IconUrl, Description);
            return badge;
        }

        public Result<Updated> Update(string IconUrl)
        {
            iconUrl = IconUrl;
            UpdatedAt = DateTime.UtcNow;

            return Result.Updated;
        }
    }
}
