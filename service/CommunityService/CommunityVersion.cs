using System.Reflection;

namespace Tudormobile.CommunityService;

/// <summary>
/// Represents version and metadata information for the Community Service.
/// </summary>
public record CommunityVersion
{
    private static readonly Lazy<string> _version = new(() =>
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        return v == null ? "0.0.0" : $"{v.Major}.{v.Minor}.{v.Build}";
    });

    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    public string Name => "CommunityService";

    /// <summary>
    /// Gets a description of the service functionality.
    /// </summary>
    public string Description => "Web services API layer for Community applications";

    /// <summary>
    /// Gets the copyright notice for the service.
    /// </summary>
    public string Copyright => "COPYRIGHT(C)2026 BILL TUDOR";

    /// <summary>
    /// Gets the version number of the service assembly.
    /// </summary>
    public string Version => _version.Value;
}
