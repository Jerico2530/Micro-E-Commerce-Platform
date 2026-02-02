using System.Security.Claims;

namespace OrderServices.Application.Helpers
{
    public static class HttpContextExtensions
    {
        public static int GetUsuarioId(this ClaimsPrincipal user)
        {
            var claimValue = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claimValue) || !int.TryParse(claimValue, out int usuarioId))
                return 0;
            return usuarioId;
        }

        public static string GetBearerToken(this HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }
            return null;
        }
    }
}
