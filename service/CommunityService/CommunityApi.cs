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

    internal Task<IResult> GetVersionAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetVersionAsync), async () =>
            Results.Ok(CommunityResponse.Success(new CommunityVersion())));

    internal Task<IResult> GetStreetsAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetStreetsAsync), async () =>
        {
            var filePath = Path.Combine(_env.ContentRootPath, "Assets", "streets.json");
            var content = await File.ReadAllTextAsync(filePath);
            var streets = JsonSerializer.Deserialize<List<string>>(content, _jsonOptions);
            return Results.Ok(CommunityResponse.Success(streets));
        });

    internal Task<IResult> QueryLocationAsync(HttpContext context, string apiKey, string street)
        => HandleApiRequest(context, apiKey, nameof(QueryLocationAsync), async () =>
        {
            var locations = _locations ??= await ReadLocationsAsync();
            return Results.Ok(new CommunityMatchCollection(locations.Take(12)));
        });

    internal Task<IResult> QueryLocationAsync(HttpContext context, string apiKey, double lat, double lng)
        => HandleApiRequest(context, apiKey, nameof(QueryLocationAsync), async () =>
        {
            var locations = _locations ??= await ReadLocationsAsync();
            return Results.Ok(new CommunityMatchCollection(locations.Take(2)));
        });

    private async Task<IResult> HandleApiRequest(HttpContext context, string apiKey, string callerName, Func<Task<IResult>> onAuthorized)
    {
        LogApiRequest(context, callerName);
        if (_isDevelopment || apiKey == _apiKey)
        {
            return await onAuthorized();
        }
        _logger.LogError("CommunityService, {caller}, {ip}, {key}, INVALID API KEY", callerName, context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();
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
