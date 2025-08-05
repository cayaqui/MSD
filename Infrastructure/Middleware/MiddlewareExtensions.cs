using Microsoft.AspNetCore.Builder;

namespace Infrastructure.Middleware
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseUserValidation(this IApplicationBuilder app)
        {
            return app.UseMiddleware<UserValidationMiddleware>();
        }
    }
}