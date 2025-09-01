using BookRental.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace BookRental.Middleware;

public class WebSocketHandlerMiddleware(RequestDelegate next, IOptionsMonitor<JwtBearerOptions> jwtOptions)
{
    private readonly RequestDelegate _next = next;
    private readonly IOptionsMonitor<JwtBearerOptions> _jwtOptions = jwtOptions;

    public async Task InvokeAsync(HttpContext context, WebSocketManager wsManager)
    {
        if (context.Request.Path != "/ws")
        {
            await _next(context);
            return;
        }

        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var authHeader = context.Request.Headers.Authorization.ToString();
        string? token = null;

        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            token = authHeader["Bearer ".Length..].Trim();
        }
        else if (context.Request.Query.ContainsKey("token"))
        {
            token = context.Request.Query["token"];
        }

        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = 401;
            return;
        }

        var validationParameters = _jwtOptions
            .Get(JwtBearerDefaults.AuthenticationScheme)
            .TokenValidationParameters;

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, validationParameters, out var validatedToken);

            var userId = principal.GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                context.Response.StatusCode = 401;
                return;
            }

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

            wsManager.AddSocket(/* userId,*/ webSocket);

            var buffer = new byte[1024 * 4];

            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            wsManager.RemoveSocket(/* userId,*/ webSocket);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
        catch (SecurityTokenException)
        {
            context.Response.StatusCode = 401;
        }
    }
}
