namespace Web.Helpers
{
    public static class ImageHelper
    {
        // Imagen placeholder en base64 (1x1 pixel transparente)
        public const string PlaceholderDataUrl = "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='150' height='150' viewBox='0 0 150 150'%3E%3Crect width='150' height='150' fill='%23f0f0f0'/%3E%3Ctext x='50%25' y='50%25' text-anchor='middle' dy='.3em' fill='%23999' font-family='Arial' font-size='14'%3ENo Image%3C/text%3E%3C/svg%3E";

        public static string GetImageUrl(string? imageUrl, string fallback = "")
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return string.IsNullOrEmpty(fallback) ? PlaceholderDataUrl : fallback;
            }

            return imageUrl;
        }

        public static string GetUserAvatarUrl(string? avatarUrl, string userName = "Usuario")
        {
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                return avatarUrl;
            }

            // Generar avatar con iniciales
            var initials = GetInitials(userName);
            return GenerateAvatarDataUrl(initials);
        }

        private static string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "U";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return $"{parts[0][0]}{parts[1][0]}".ToUpper();
            }

            return name[0].ToString().ToUpper();
        }

        private static string GenerateAvatarDataUrl(string initials)
        {
            var svg = $@"data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='40' height='40' viewBox='0 0 40 40'%3E%3Ccircle cx='20' cy='20' r='20' fill='%234f46e5'/%3E%3Ctext x='50%25' y='50%25' text-anchor='middle' dy='.3em' fill='white' font-family='Arial' font-size='16' font-weight='600'%3E{initials}%3C/text%3E%3C/svg%3E";
            return svg;
        }
    }
}