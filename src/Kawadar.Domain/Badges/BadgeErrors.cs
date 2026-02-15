using Kawadar.Domain.Common.Results;

namespace Kawadar.Domain.Badges
{
    public class BadgeErrors
    {
        public static Error TitleIsEmpty => Error.Validation("Badge.TitleIsEmpty",
            "Title Can't be empty");

        public static Error IconIsEmpty => Error.Validation("Badge.IconIsEmpty",
            "Icon Url can't be empty");
    }
}
