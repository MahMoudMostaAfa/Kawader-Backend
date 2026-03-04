namespace Kawadar.Application.Common.ExtensionValidator
{
    public static class Extensions
    {
        public static readonly string[] AllowedImageExtensions = { ".png", ".jpeg", ".jpg" };

        public static readonly string[] AllowedJobAttachmentExtensions =
        {
            ".png", ".jpeg", ".jpg",   // images
            ".pdf",                       // PDF
            ".zip", ".rar", ".7z"        // archives
        };
    }
}
