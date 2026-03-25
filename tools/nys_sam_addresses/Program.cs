using System.Text.Json;

class Program
{
    // Feature Service endpoint for NYS Address Points
    private const string BaseUrl = "https://services6.arcgis.com/EbVsqZ18sv1kVJ3k/arcgis/rest/services/NYS_Address_Points/FeatureServer/0/query";

    static async Task Main(string[] args)
    {
        using HttpClient client = new HttpClient();
        List<JsonElement> allFeatures = new List<JsonElement>();

        int offset = 0;
        int pageSize = 1000; // Standard ArcGIS maxRecordCount limit
        bool hasMore = true;

        Console.WriteLine("Starting download for Clifton Park...about 18,172 records expected.");

        while (hasMore)
        {
            // Build the query string with pagination parameters
            // Field 'CityTownName' is used for filtering by municipality
            string query = $"?where=CityTownName='CLIFTON PARK'" +
                           $"&outFields=*" +
                           $"&outSR=4326" + // WGS84 for GeoJSON compatibility
                           $"&resultOffset={offset}" +
                           $"&resultRecordCount={pageSize}" +
                           $"&f=geojson";

            // See: https://data.gis.ny.gov/datasets/sharegisny::nys-address-points/api
            // to develop full query parameters for filtering by municipality, etc.
            var fullUrl = "https://services6.arcgis.com/EbVsqZ18sv1kVJ3k/arcgis/rest/services/NYS_Address_Points/FeatureServer/0/query?where=CityTownName%20%3D%20'CLIFTON%20PARK'&outFields=CompleteStreetName,StreetName,AddressNumber,AddressLabel,ZipCode,CityTownName&outSR=4326&f=json";
            fullUrl += $"&resultOffset={offset}&resultRecordCount={pageSize}";
            var response = await client.GetAsync(fullUrl);
            var content = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;

            // Extract features from the current page
            if (root.TryGetProperty("features", out var features))
            {
                foreach (var feature in features.EnumerateArray())
                {
                    allFeatures.Add(feature.Clone());
                }
                Console.WriteLine($"Fetched {allFeatures.Count} records...");
            }

            // Check if the server indicates more records exist
            if (root.TryGetProperty("exceededTransferLimit", out var limitReached))
            {
                hasMore = limitReached.GetBoolean();
                offset += pageSize;
            }
            else
            {
                hasMore = false;
            }
        }

        // Wrap features into a valid GeoJSON FeatureCollection
        var geoJsonOutput = new
        {
            type = "FeatureCollection",
            features = allFeatures
        };

        string finalJson = JsonSerializer.Serialize(geoJsonOutput, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync("clifton_park_addresses.geojson", finalJson);

        Console.WriteLine($"\nDownload complete. Total records: {allFeatures.Count}");
        Console.WriteLine("File saved: clifton_park_addresses.geojson");

        // Converting records to smaller entities.

        // Unique street names in Clifton Park
        Console.Write("\nUnique street names in Clifton Park:");
        var completeStreetNames = allFeatures.Select(f => f.GetProperty("attributes").GetProperty("CompleteStreetName").GetString()).ToList();
        var uniqueStreetNames = completeStreetNames.Distinct().OrderBy(name => name).ToArray();
        Console.WriteLine(uniqueStreetNames.Length.ToString("N0"));

        string streetsJson = JsonSerializer.Serialize(uniqueStreetNames, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync("clifton_park_streets.json", streetsJson);

        Console.WriteLine("File saved: clifton_park_streets.json");

        // Convert to smaller entities (AddressPoint)
        var addressPoints = allFeatures.Select(f => new AddressPoint
        {
            Adr = f.GetProperty("attributes").GetProperty("AddressLabel")!.GetString()!,
            Lat = f.GetProperty("geometry").GetProperty("y").GetDouble(),
            Lng = f.GetProperty("geometry").GetProperty("x").GetDouble()
        }).ToList();
        Console.WriteLine($"\nConverted to (AddressPoint). Total: {addressPoints.Count.ToString("N0")}");
        string addressPointsJson = JsonSerializer.Serialize(addressPoints, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await File.WriteAllTextAsync("clifton_park_address_points.json", addressPointsJson);
        Console.WriteLine("File saved: clifton_park_address_points.json");
    }
}

public class AddressPoint
{
    public string Adr { get; set; } = string.Empty;
    public double Lat { get; set; }
    public double Lng { get; set; }
}
