using System.Reflection;

namespace Tudormobile.CommunityService;

public record CommunityVersion
{
    private static Lazy<string> _version = new Lazy<string>(() =>
    {
        var v = Assembly.GetExecutingAssembly().GetName().Version;
        return v == null ? "0.0.0" : $"{v.Major}.{v.Minor}.{v.Build}";
    });
    public string Name => "CommunityService";
    public string Description => "Web services API layer for Community applications";
    public string Copyright => "COPYRIGHT(C)2026 BILL TUDOR";
    public string Version => _version.Value;
}
