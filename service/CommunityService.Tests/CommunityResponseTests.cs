namespace CommunityService.Tests;

[TestClass]
public class CommunityResponseTests
{
    [TestMethod]
    public void Constructor_Initializes()
    {
        var response = new CommunityResponse<string>();

        Assert.IsNull(response.Data);
        Assert.IsFalse(response.IsSuccess);
    }

    [TestMethod]
    public void Constructor_WithProperties_Initializes()
    {
        var data = new object();
        var success = true;
        var response = new CommunityResponse<object>()
        {
            IsSuccess = success,
            Data = data
        };

        Assert.AreEqual(data, response.Data);
        Assert.IsTrue(response.IsSuccess);
    }

    [TestMethod]
    public void Success_SetsSuccess()
    {
        var data = "somedata";
        var response = CommunityResponse.Success(data);

        Assert.IsTrue(response.IsSuccess);
        Assert.AreEqual(data, response.Data);
    }

    [TestMethod]
    public void Failure_SetsFailure()
    {
        object? data = null;
        var response = CommunityResponse.Failure(data);

        Assert.IsFalse(response.IsSuccess);
        Assert.IsNull(response.Data);
    }

}
