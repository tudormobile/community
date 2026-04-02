using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Tudormobile.CommunityService;

internal class CommunityApi
{
    private readonly string _apiKey;
    private readonly ILogger _logger;
    private readonly IWebHostEnvironment _env;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly Lazy<Task<IReadOnlyList<CommunityLocation>>> _locationsLazy;

    public CommunityApi(string apiKey, ILogger logger, IWebHostEnvironment env, JsonSerializerOptions jsonOptions)
    {
        _apiKey = apiKey;
        _logger = logger;
        _env = env;
        _jsonOptions = jsonOptions;
        _locationsLazy = new Lazy<Task<IReadOnlyList<CommunityLocation>>>(LoadLocationsAsync);
    }

    internal Task<IResult> GetVersionAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetVersionAsync), async () =>
            Results.Ok(CommunityResponse.Success(new CommunityVersion())));

    internal Task<IResult> GetStreetsAsync(HttpContext context, string apiKey)
        => HandleApiRequest(context, apiKey, nameof(GetStreetsAsync), async () =>
        {
            try
            {
                var filePath = Path.Combine(_env.ContentRootPath, "Assets", "streets.json");
                var content = await File.ReadAllTextAsync(filePath);
                var streets = JsonSerializer.Deserialize<List<string>>(content, _jsonOptions);
                return Results.Ok(CommunityResponse.Success(streets));
            }
            catch (Exception ex)
            {
                return Results.Ok(CommunityResponse.Failure(ex.Message));
            }
        });

    internal Task<IResult> QueryLocationAsync(HttpContext context, string apiKey, string street)
        => HandleApiRequest(context, apiKey, nameof(QueryLocationAsync), async () =>
        {
            try
            {
                var locations = await _locationsLazy.Value;

                if (string.IsNullOrWhiteSpace(street))
                {
                    return Results.Ok(new CommunityMatchCollection([]));
                }

                var searchTerm = street.Trim();
                var exactMatches = locations
                    .Where(l => l.Adr.Equals(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (exactMatches.Count > 0)
                {
                    return Results.Ok(new CommunityMatchCollection(exactMatches, isExactMatch: true));
                }

                var partialMatches = locations
                    .Where(l => l.Adr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Take(12)
                    .ToList();

                return Results.Ok(new CommunityMatchCollection(partialMatches, isExactMatch: false));
            }
            catch (Exception ex)
            {
                return Results.Ok(CommunityResponse.Failure(ex.Message));
            }
        });

    internal Task<IResult> QueryLocationAsync(HttpContext context, string apiKey, double lat, double lng)
        => HandleApiRequest(context, apiKey, nameof(QueryLocationAsync), async () =>
        {
            try
            {
                var locations = await _locationsLazy.Value;

                var nearbyLocations = locations
                    .Select(l => new
                    {
                        Location = l,
                        Distance = CalculateDistance(lat, lng, l.Lat, l.Lng)
                    })
                    .OrderBy(x => x.Distance)
                    .Take(12)
                    .Select(x => x.Location)
                    .ToList();

                var isExact = nearbyLocations.Any(l => Math.Abs(l.Lat - lat) < 0.0001 && Math.Abs(l.Lng - lng) < 0.0001);
                return Results.Ok(new CommunityMatchCollection(nearbyLocations, isExactMatch: isExact));
            }
            catch (Exception ex)
            {
                return Results.Ok(CommunityResponse.Failure(ex.Message));
            }
        });

    private async Task<IResult> HandleApiRequest(HttpContext context, string apiKey, string callerName, Func<Task<IResult>> onAuthorized)
    {
        LogApiRequest(context, callerName);
        if (apiKey == _apiKey)
        {
            return await onAuthorized();
        }
        _logger.LogError("CommunityService, {caller}, {ip}, {key}, INVALID API KEY", callerName, context.Connection.RemoteIpAddress, apiKey);
        return Results.NotFound();
    }

    private void LogApiRequest(HttpContext context, [CallerMemberName] string callerName = "")
    {
        _logger.LogInformation("CommunityService, {callerName}, {remoteIpAddress}",
            callerName, context.Connection.RemoteIpAddress);
    }

    private async Task<IReadOnlyList<CommunityLocation>> LoadLocationsAsync()
    {
        var filePath = Path.Combine(_env.ContentRootPath, "Assets", "locations.json");
        using var stream = File.OpenRead(filePath);
        var locations = await JsonSerializer.DeserializeAsync<List<CommunityLocation>>(stream, _jsonOptions);
        return locations ?? [];
    }

    private static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
    {
        const double earthRadiusKm = 6371.0;
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLng = DegreesToRadians(lng2 - lng1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

}
