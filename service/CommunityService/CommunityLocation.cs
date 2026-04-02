namespace Tudormobile.CommunityService;

/// <summary>
/// Represents a geographic location within a community.
/// </summary>
public record CommunityLocation
{
    /// <summary>
    /// Gets the address of the location.
    /// </summary>
    public string Adr { get; init; } = string.Empty;

    /// <summary>
    /// Gets the latitude coordinate of the location.
    /// </summary>
    public double Lat { get; init; }

    /// <summary>
    /// Gets the longitude coordinate of the location.
    /// </summary>
    public double Lng { get; init; }
}
