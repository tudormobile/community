using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Tudormobile.CommunityService;

internal class CommunityApi
{
    private string _apiKey;
    private readonly ILogger _logger;
    private readonly bool _isDevelopment;
    private readonly IWebHostEnvironment _env;
    private readonly JsonSerializerOptions _jsonOptions;
    private static IReadOnlyList<CommunityLocation>? _locations;

    public CommunityApi(string apiKey, ILogger logger, IWebHostEnvironment env, JsonSerializerOptions jsonOptions)
    {
        _apiKey = apiKey;
        _logger = logger;
        _env = env;
        _isDevelopment = env.IsDevelopment();
        _jsonOptions = jsonOptions;
    }

    internal async Task<IResult> GetVersionAsync(HttpContext context, string apiKey)
    {
        LogApiRequest(context);
        if (_isDevelopment || apiKey == _apiKey)
        {
            return Results.Ok(CommunityResponse.IsSuccess(new CommunityVersion()));
        }
        _logger.LogError("CommunityService, {caller}, {ip}, {key}, INVALID API KEY", nameof(GetVersionAsync), context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();  // Do NOT return regular response; when API KEY is always indicate nothing ever found
    }

    internal async Task<IResult> GetStreetsAsync(HttpContext context, string apiKey)
    {
        LogApiRequest(context);
        if (_isDevelopment || apiKey == _apiKey)
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Assets", "streets.json");
            var content = await File.ReadAllTextAsync(filePath);
            var streets = JsonSerializer.Deserialize<List<string>>(content, _jsonOptions);
            return Results.Ok(CommunityResponse.IsSuccess(streets));
        }
        _logger.LogError("CommunityService, {caller}, {ip}, {key}, INVALID API KEY", nameof(GetStreetsAsync), context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();  // Do NOT return regular response; when API KEY is always indicate nothing ever found
    }

    internal async Task<IResult> QueryLocationAsync(HttpContext context, string apiKey, string street)
    {
        LogApiRequest(context);
        if (_isDevelopment || apiKey == _apiKey)
        {
            var locations = _locations ??= await ReadLocationsAsync();
            return Results.Ok(new CommunityMatchCollection(locations.Take(12)));
        }
        _logger.LogError("CommunityService, {caller}, {ip}, {key}, INVALID API KEY", nameof(QueryLocationAsync), context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();  // Do NOT return regular response; when API KEY is always indicate nothing ever found
    }

    internal async Task<IResult> QueryLocationAsync(HttpContext context, string apiKey, double lat, double lng)
    {
        LogApiRequest(context);
        if (_isDevelopment || apiKey == _apiKey)
        {
            var locations = _locations ??= await ReadLocationsAsync();
            return Results.Ok(new CommunityMatchCollection(locations.Take(2)));
        }
        _logger.LogError("CommunityService, {caller}, {ip}, {key}, INVALID API KEY", nameof(QueryLocationAsync), context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();  // Do NOT return regular response; when API KEY is always indicate nothing ever found
    }

    private void LogApiRequest(HttpContext context, [CallerMemberName] string callerName = "")
    {
        var message = $"CommunityService, {callerName}, {context.Connection.RemoteIpAddress}";
        Debug.WriteLine(message);
        _logger.LogInformation(message);
    }

    private async Task<List<CommunityLocation>> ReadLocationsAsync()
    {
        var filePath = Path.Combine(_env.ContentRootPath, "Assets", "locations.json");
        using var stream = File.OpenRead(filePath);
        var locations = await JsonSerializer.DeserializeAsync<List<CommunityLocation>>(stream, _jsonOptions);
        return locations ?? [];
    }

}
