using NutvaCms.Application.Interfaces;
namespace NutvaCms.API.Middlewares;
public class JwtBlacklistMiddleware
{
    private readonly RequestDelegate _next;

    public JwtBlacklistMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ITokenBlacklistService blacklistService)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        if (!string.IsNullOrEmpty(token) && blacklistService.IsTokenBlacklisted(token))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Token has been revoked.");
            return;
        }

        await _next(context);
    }
}
