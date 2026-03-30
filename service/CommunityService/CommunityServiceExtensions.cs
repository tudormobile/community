using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Tudormobile.Dbx;

namespace Tudormobile.CommunityService;

public static class CommunityServiceExtensions
{
    public static WebApplication UseCommunityService(this WebApplication app)
    {
        var prefix = "/map/community/v1";
        app.UseDbx(prefix);

        // Get the JsonOptions from DI
        var jsonOptions = app.Services.GetRequiredService<IOptions<JsonOptions>>();

        var api = new CommunityApi(
            app.Configuration.GetSection("Community")["ApiKey"] ?? String.Empty,
            app.Logger,
            app.Environment, jsonOptions?.Value.JsonSerializerOptions ?? new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Map community endpoints
        app.MapGet($"{prefix}/status", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey)
            => api.GetVersionAsync(context, apiKey ?? string.Empty));

        app.MapGet($"{prefix}/streets", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey)
            => api.GetStreetsAsync(context, apiKey ?? string.Empty));

        app.MapGet($"{prefix}/query", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey, [FromQuery] string street)
            => api.QueryLocationAsync(context, apiKey ?? string.Empty, street));

        app.MapGet($"{prefix}/location", (HttpContext context, [FromHeader(Name = "ApiKey")] string? apiKey, [FromQuery] double lat, [FromQuery] double lng)
            => api.QueryLocationAsync(context, apiKey ?? string.Empty, lat, lng));

        app.Logger.LogInformation("CommunityService, Running");
        return app;
    }
}
