namespace CommunityService.Tests;

[TestClass]
public class CommunityVersionTests
{
    [TestMethod]
    public void Construct_SetsDefaultValues()
    {
        var version = new CommunityVersion();

        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Name));
        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Description));
        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Copyright));
        Assert.IsFalse(string.IsNullOrWhiteSpace(version.Version));
    }
}