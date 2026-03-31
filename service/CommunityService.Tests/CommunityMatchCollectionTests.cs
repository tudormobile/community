namespace CommunityService.Tests;

[TestClass]
public class CommunityMatchCollectionTests
{
    [TestMethod]
    public void Constructor_Initializes()
    {
        var matches = new CommunityMatchCollection();

        Assert.IsNotNull(matches.Matches);
        Assert.IsFalse(matches.IsExact);
        Assert.IsEmpty(matches.Matches);
    }

    [TestMethod]
    public void Construct_WithParameters_Initializes()
    {
        var isExact = true;
        var matches = new List<CommunityLocation>()
        {
            new CommunityLocation(),
            new CommunityLocation()
        };

        var collection = new CommunityMatchCollection(matches, isExact);

        Assert.IsTrue(collection.IsExact);
        Assert.HasCount(2, collection.Matches);
        Assert.Contains(matches[0], collection.Matches);
        Assert.Contains(matches[1], collection.Matches);
    }
}
