namespace Tudormobile.CommunityService;

public record CommunityLocation
{
    public string Adr { get; init; } = string.Empty;
    public double Lat { get; init; }
    public double Lng { get; init; }
}
