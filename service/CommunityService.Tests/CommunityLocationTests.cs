namespace CommunityService.Tests;

[TestClass]
public class CommunityLocationTests
{
    //    public string Adr { get; init; } = string.Empty;
    //    public double Lat { get; init; }
    //    public double Lng { get; init; }
    [TestMethod]
    public void Construct_SetsDefaultValues()
    {
        var location = new CommunityLocation();

        Assert.IsNotNull(location.Adr);
        Assert.AreEqual(0, location.Lat);
        Assert.AreEqual(0, location.Lng);
    }

    [TestMethod]
    public void ConstructWithValues_Contructs()
    {
        var lat = 1.234;
        var lng = 5.678;
        var adr = "some location";

        var location = new CommunityLocation()
        {
            Adr = adr,
            Lat = lat,
            Lng = lng,
        };

        Assert.AreEqual(adr, location.Adr);
        Assert.AreEqual(lat, location.Lat);
        Assert.AreEqual(lng, location.Lng);

    }
}
