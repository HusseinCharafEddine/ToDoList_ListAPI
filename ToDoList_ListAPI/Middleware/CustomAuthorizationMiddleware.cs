using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ToDoList_ListAPI.Middleware
{
    public class CustomAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomAuthorizationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if the user is authenticated
            if (context.User.Identity.IsAuthenticated)
            {
                // Perform custom authorization logic here
                // For demonstration purposes, let's assume a role-based authorization check
                if (context.User.IsInRole("Admin"))
                {
                    // User is authorized, continue with the request pipeline
                    await _next(context);
                }
                else
                {
                    // User is not authorized, return a 403 Forbidden response
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Access Forbidden. User lacks required role.");
                }
            }
            else
            {
                // User is not authenticated, return a 401 Unauthorized response
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized. Please log in.");
            }
        }
    }

    public static class CustomAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomAuthorizationMiddleware>();
        }
    }
}
