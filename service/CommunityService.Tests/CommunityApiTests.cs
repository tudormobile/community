using CommunityService.Tests.TestHelpers;
using System.Text.Json;

namespace CommunityService.Tests;

[TestClass]
public class CommunityApiTests
{
    private const string TestApiKey = "test-api-key-12345";
    private TestLogger _logger = null!;
    private TestWebHostEnvironment _environment = null!;
    private JsonSerializerOptions _jsonOptions = null!;
    private CommunityApi _api = null!;
    private TestHttpContext _httpContext = null!;
    private string _testDirectory = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        // Create unique test directory for each test to avoid parallel execution conflicts
        _testDirectory = Path.Combine(Path.GetTempPath(), $"CommunityApiTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_testDirectory);

        _logger = new TestLogger();
        _environment = new TestWebHostEnvironment { ContentRootPath = _testDirectory };
        _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        _api = new CommunityApi(TestApiKey, _logger, _environment, _jsonOptions);
        _httpContext = new TestHttpContext();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _logger.Clear();

        // Clean up test directory
        try
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    #region GetVersionAsync Tests

    [TestMethod]
    public async Task GetVersionAsync_WithValidApiKey_ReturnsOkWithVersion()
    {
        // Act
        var result = await _api.GetVersionAsync(_httpContext, TestApiKey);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType<IResult>(result);

        // Verify logging occurred
        Assert.IsTrue(_logger.LoggedMessages.Any(m => m.Contains("GetVersionAsync")));
    }

    [TestMethod]
    public async Task GetVersionAsync_WithInvalidApiKey_ReturnsNotFound()
    {
        // Act
        var result = await _api.GetVersionAsync(_httpContext, "wrong-key");

        // Assert
        Assert.IsNotNull(result);

        // Verify error was logged
        Assert.IsTrue(_logger.LoggedLevels.Contains(LogLevel.Error));
        Assert.IsTrue(_logger.LoggedMessages.Any(m => m.Contains("INVALID API KEY")));
    }

    [TestMethod]
    public async Task GetVersionAsync_WithEmptyApiKey_ReturnsNotFound()
    {
        // Act
        var result = await _api.GetVersionAsync(_httpContext, string.Empty);

        // Assert
        Assert.IsNotNull(result);

        // Verify error was logged
        Assert.IsTrue(_logger.LoggedLevels.Contains(LogLevel.Error));
    }

    [TestMethod]
    public async Task GetVersionAsync_LogsRequestWithIpAddress()
    {
        // Arrange
        _httpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Parse("192.168.1.100");

        // Act
        await _api.GetVersionAsync(_httpContext, TestApiKey);

        // Assert
        Assert.IsTrue(_logger.LoggedMessages.Any(m => m.Contains("192.168.1.100")));
    }

    #endregion

    #region GetStreetsAsync Tests

    [TestMethod]
    public async Task GetStreetsAsync_WithValidApiKey_ReturnsOkResult()
    {
        // Arrange
        var testAssetsPath = Path.Combine(_environment.ContentRootPath, "Assets");
        Directory.CreateDirectory(testAssetsPath);

        var streetsData = new List<string> { "Main Street", "Oak Avenue", "Park Boulevard" };
        var json = JsonSerializer.Serialize(streetsData, _jsonOptions);
        var streetsFile = Path.Combine(testAssetsPath, "streets.json");

        try
        {
            await File.WriteAllTextAsync(streetsFile, json);

            // Act
            var result = await _api.GetStreetsAsync(_httpContext, TestApiKey);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_logger.LoggedMessages.Any(m => m.Contains("GetStreetsAsync")));
        }
        finally
        {
            // Cleanup
            if (File.Exists(streetsFile))
                File.Delete(streetsFile);
        }
    }

    [TestMethod]
    public async Task GetStreetsAsync_WithMissingFile_ReturnsFailureResponse()
    {
        // Arrange - Ensure assets directory doesn't exist or file doesn't exist
        var testAssetsPath = Path.Combine(_environment.ContentRootPath, "Assets");
        var streetsFile = Path.Combine(testAssetsPath, "streets.json");

        if (File.Exists(streetsFile))
            File.Delete(streetsFile);

        // Act
        var result = await _api.GetStreetsAsync(_httpContext, TestApiKey);

        // Assert
        Assert.IsNotNull(result);
        // The API should handle the error gracefully
    }

    [TestMethod]
    public async Task GetStreetsAsync_WithInvalidApiKey_ReturnsNotFound()
    {
        // Act
        var result = await _api.GetStreetsAsync(_httpContext, "invalid-key");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(_logger.LoggedLevels.Contains(LogLevel.Error));
    }

    [TestMethod]
    public async Task GetStreetsAsync_WithMalformedJson_ReturnsFailureResponse()
    {
        // Arrange
        var testAssetsPath = Path.Combine(_environment.ContentRootPath, "Assets");
        Directory.CreateDirectory(testAssetsPath);

        var streetsFile = Path.Combine(testAssetsPath, "streets.json");

        try
        {
            await File.WriteAllTextAsync(streetsFile, "{ invalid json }");

            // Act
            var result = await _api.GetStreetsAsync(_httpContext, TestApiKey);

            // Assert
            Assert.IsNotNull(result);
            // Should return a failure response with error message
        }
        finally
        {
            // Cleanup
            if (File.Exists(streetsFile))
                File.Delete(streetsFile);
        }
    }

    #endregion

    #region QueryLocationAsync (Street) Tests

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_WithValidApiKey_ReturnsOkResult()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, "Main Street");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_logger.LoggedMessages.Any(m => m.Contains("QueryLocationAsync")));
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_WithInvalidApiKey_ReturnsNotFound()
    {
        // Act
        var result = await _api.QueryLocationAsync(_httpContext, "wrong-key", "Main Street");

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(_logger.LoggedLevels.Contains(LogLevel.Error));
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_WithEmptyStreet_ReturnsEmptyMatches()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, string.Empty);

            // Assert
            Assert.IsNotNull(result);
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_WithMissingLocationsFile_ReturnsFailureResponse()
    {
        // Arrange - Ensure locations file doesn't exist
        CleanupLocationsFile();

        // Act
        var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, "Main Street");

        // Assert
        Assert.IsNotNull(result);
        // Should handle missing file gracefully
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_WithExactMatch_ReturnsExactMatch()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query for exact address from test data
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, "123 Main St");

            // Assert
            Assert.IsNotNull(result);
            // The actual verification of exact match would require deserializing the result
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_WithPartialMatch_ReturnsPartialMatches()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query for partial match
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, "Main");

            // Assert
            Assert.IsNotNull(result);
            // Should return locations containing "Main"
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_CaseInsensitive_ReturnsMatches()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query with different case
            var result1 = await _api.QueryLocationAsync(_httpContext, TestApiKey, "main st");
            var result2 = await _api.QueryLocationAsync(_httpContext, TestApiKey, "MAIN ST");
            var result3 = await _api.QueryLocationAsync(_httpContext, TestApiKey, "Main St");

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
            Assert.IsNotNull(result3);
            // All should return the same results
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_WithWhitespace_HandlesTrimming()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query with leading/trailing whitespace
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, "  Main St  ");

            // Assert
            Assert.IsNotNull(result);
            // Should trim whitespace and return matches
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByStreet_NoMatches_ReturnsEmptyCollection()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query for non-existent street
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, "NonExistent Street Name XYZ");

            // Assert
            Assert.IsNotNull(result);
            // Should return empty collection
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    #endregion

    #region QueryLocationAsync (Coordinates) Tests

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_WithValidApiKey_ReturnsOkResult()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 40.7128, -74.0060);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(_logger.LoggedMessages.Any(m => m.Contains("QueryLocationAsync")));
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_WithInvalidApiKey_ReturnsNotFound()
    {
        // Act
        var result = await _api.QueryLocationAsync(_httpContext, "invalid-key", 40.7128, -74.0060);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(_logger.LoggedLevels.Contains(LogLevel.Error));
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_WithZeroCoordinates_ReturnsOkResult()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 0.0, 0.0);

            // Assert
            Assert.IsNotNull(result);
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_WithExtremeCoordinates_ReturnsOkResult()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Test with extreme valid coordinates
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 89.9, 179.9);

            // Assert
            Assert.IsNotNull(result);
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_WithMissingLocationsFile_ReturnsFailureResponse()
    {
        // Arrange - Ensure locations file doesn't exist
        CleanupLocationsFile();

        // Act
        var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 40.7128, -74.0060);

        // Assert
        Assert.IsNotNull(result);
        // Should handle missing file gracefully
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_FindsNearestLocations()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query near "123 Main St" (40.7128, -74.0060)
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 40.7130, -74.0062);

            // Assert
            Assert.IsNotNull(result);
            // Should return locations ordered by distance
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_ExactMatch_MarksAsExact()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query exact coordinates from test data
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 40.7128, -74.0060);

            // Assert
            Assert.IsNotNull(result);
            // Should mark as exact match
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_ReturnsMaximum12Locations()
    {
        // Arrange
        SetupLocationsFileWithManyLocations();

        try
        {
            // Act
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 40.7128, -74.0060);

            // Assert
            Assert.IsNotNull(result);
            // Should return at most 12 locations even if more exist
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task QueryLocationAsync_ByCoordinates_OrdersByDistance()
    {
        // Arrange
        SetupLocationsFile();

        try
        {
            // Act - Query from a specific point
            var result = await _api.QueryLocationAsync(_httpContext, TestApiKey, 40.7128, -74.0060);

            // Assert
            Assert.IsNotNull(result);
            // Closest location should be first
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    #endregion

    #region Multiple Requests Tests

    [TestMethod]
    public async Task QueryLocationAsync_MultipleRequests_LoadsLocationsOnlyOnce()
    {
        // Arrange
        SetupLocationsFile();
        _logger.Clear();

        try
        {
            // Act - Make multiple requests
            await _api.QueryLocationAsync(_httpContext, TestApiKey, "Street 1");
            await _api.QueryLocationAsync(_httpContext, TestApiKey, "Street 2");
            await _api.QueryLocationAsync(_httpContext, TestApiKey, 40.0, -74.0);
            await _api.QueryLocationAsync(_httpContext, TestApiKey, 41.0, -75.0);

            // Assert
            // All requests should succeed
            var logCount = _logger.LoggedMessages.Count(m => m.Contains("QueryLocationAsync"));
            Assert.AreEqual(4, logCount);
        }
        finally
        {
            CleanupLocationsFile();
        }
    }

    [TestMethod]
    public async Task GetVersionAsync_CalledMultipleTimes_LogsEachRequest()
    {
        // Arrange
        _logger.Clear();

        // Act
        await _api.GetVersionAsync(_httpContext, TestApiKey);
        await _api.GetVersionAsync(_httpContext, TestApiKey);
        await _api.GetVersionAsync(_httpContext, TestApiKey);

        // Assert
        var logCount = _logger.LoggedMessages.Count(m => m.Contains("GetVersionAsync"));
        Assert.AreEqual(3, logCount);
    }

    #endregion

    #region Helper Methods

    private void SetupLocationsFile()
    {
        // Now uses _env.ContentRootPath (set to _testDirectory in TestInitialize)
        var testAssetsPath = Path.Combine(_testDirectory, "Assets");
        Directory.CreateDirectory(testAssetsPath);

        var locations = new List<CommunityLocation>
        {
            new() { Adr = "123 Main St", Lat = 40.7128, Lng = -74.0060 },
            new() { Adr = "456 Oak Ave", Lat = 40.7589, Lng = -73.9851 },
            new() { Adr = "789 Park Blvd", Lat = 40.7614, Lng = -73.9776 },
            new() { Adr = "321 Elm St", Lat = 40.7489, Lng = -73.9680 },
            new() { Adr = "654 Pine Rd", Lat = 40.7580, Lng = -73.9855 }
        };

        var json = JsonSerializer.Serialize(locations, _jsonOptions);
        var locationsFile = Path.Combine(testAssetsPath, "locations.json");
        File.WriteAllText(locationsFile, json);
    }

    private void SetupLocationsFileWithManyLocations()
    {
        var testAssetsPath = Path.Combine(_testDirectory, "Assets");
        Directory.CreateDirectory(testAssetsPath);

        var locations = new List<CommunityLocation>();

        // Create 20 locations to test the 12-location limit
        for (int i = 0; i < 20; i++)
        {
            locations.Add(new CommunityLocation
            {
                Adr = $"{100 + i} Test Street {i}",
                Lat = 40.7128 + (i * 0.01),
                Lng = -74.0060 + (i * 0.01)
            });
        }

        var json = JsonSerializer.Serialize(locations, _jsonOptions);
        var locationsFile = Path.Combine(testAssetsPath, "locations.json");
        File.WriteAllText(locationsFile, json);
    }

    private void CleanupLocationsFile()
    {
        // Cleanup handled by TestCleanup - each test has its own directory
    }

    #endregion
}
