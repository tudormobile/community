using Microsoft.Extensions.FileProviders;

namespace CommunityService.Tests.TestHelpers;

/// <summary>
/// Simple test web host environment for unit testing.
/// </summary>
internal class TestWebHostEnvironment : IWebHostEnvironment
{
    public string WebRootPath { get; set; } = string.Empty;
    public IFileProvider WebRootFileProvider { get; set; } = null!;
    public string ApplicationName { get; set; } = "TestApp";
    public IFileProvider ContentRootFileProvider { get; set; } = null!;
    public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
    public string EnvironmentName { get; set; } = "Development";
}
