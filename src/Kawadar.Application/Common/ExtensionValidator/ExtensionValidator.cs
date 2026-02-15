namespace Kawadar.Application.Common.ExtensionValidator
{
    public static class ExtensionValidator
    {
        public static bool ValidExtension(string fileName, string[] allowedExtensions)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
    }
}
