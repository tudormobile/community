# Community Service Applications
Reference documentation for applications in the `service` folder.

## Purpose
This document provides documentation for web services API applications used by Community.

## App Index
| Service | Status | Description |
| --- | --- | --- |
| `CommunityService` | Active | Map-focused community service api layer |
---

## Service: CommunitySerivce

### Summary
CommunityService is a web services API layer that provides address completion, address validation, and marker creation/storage for mapping based applications. Useful for location-based community events where several locations are involved.

### Current Scope and Limitations
- Limited address lists, such as a single town.
- Limited security/authentication features (API-KEY based access, no users, pre-configured identifiers).
- Requires manual server configuration (API Keys and Identifiers).
- No admin console.

### Dependencies
- Runs under an existing ASPNET web services host or a separate host.
- Requires `Tudormobile.Dbx` for data storage api.

### Documentation
The following service endpoints are implemented.
| Name  | Method | Path | Description |
| ----- | ------ | ---- | ----------- |
| status| GET    | `/map/community/v1/status/` | Returns the status of the service |
| steets| GET    | `/map/community/v1/streets` | Returns list of available street names |
| query | GET    | `/map/community/v1/query?lat={lat}&lng={lng}` | Returns nearest addresses to location |
| query | GET    | `/map/community/v1/query?address={address}` | Query for matching addresses |
| dbx   | GET    | `/map/community/v1/dbx/...` | * See Dbx documentation * |

An *apikey* is expected in the API_KEY header.

### Response Codes
- **200 Ok** - returned with success or failure response message in the body; data varies
- **404 Not Found** - returned with missing api key or invalid path

#### Data Objects in Response Messages
All endpoints return a Json response message as follows:
```json
{
    "isSuccess": true|false,
    "data": { ... }
}
```
The data property is an api-specific reply or an error message. The status endpoint returns the following data:
```json
{
    "name": "CommunityService",
    "description": "Web services API layer for Community applications",
    "copyright": "COPYRIGHT(C)2026 BILL TUDOR",
    "version": "1.0.0"
}
```
For the streets endpoint, it is a simple string array of street names. For the query endpoints, it is a query result that takes the following form:
```json
{
    "exact": true|false,
    "matches": [
        { match one ... },
        { match two ... },
        ...
    ]
}
```
When the *exact* property is true, the first match is an exact match. Otherwise, matches are provided in the order of "closeness", which is either the nearest to the provided location (latitude/longitude) or the closest address match to the provided address string (address).

The match object is as follows:
```json
{
    "adr": "89 Riverview Rd",
    "lat": 42.79717898924057,
    "lng": -73.77874189662009
}
```
## Query Endpoints
### 1. Street Query - Smart Filtering

#### Features:
✅ **Empty/Whitespace Handling**
- Returns empty collection if street is null, empty, or whitespace
- Trims input to handle leading/trailing spaces

✅ **Exact Match Priority**
- First checks for exact address matches (case-insensitive)
- Returns immediately with `isExact=true` if found

✅ **Partial Match Fallback**
- If no exact match, searches for partial matches
- Uses `Contains` with case-insensitive comparison
- Limits results to 12 locations
- Returns with `isExact=false`

#### Example Behavior:
```csharp
// Exact match
"123 Main St" → Returns only "123 Main St" with isExact=true

// Partial match
"Main" → Returns all addresses containing "Main" (up to 12) with isExact=false

// Case-insensitive
"main st", "MAIN ST", "Main St" → All return same results

// Empty input
"", "   " → Returns empty collection
```

### 2. Coordinate Query - Distance-Based Search

#### Features:
✅ **Haversine Distance Calculation**
- Calculates great-circle distance between query point and all locations
- Accounts for Earth's curvature
- Returns distance in kilometers

✅ **Nearest First Ordering**
- Sorts locations by distance (nearest first)
- Returns top 12 closest locations

✅ **Exact Match Detection**
- Checks if any location is within 0.0001 degrees (~11 meters)
- Sets `isExact=true` if found

#### Algorithm:
```csharp
1. Load all locations
2. Calculate distance from (lat, lng) to each location
3. Sort by distance (ascending)
4. Take first 12 locations
5. Check if closest is within 0.0001° → isExact
6. Return sorted collection
```

### 3. Distance Calculation - Haversine Formula

#### Implementation:
```csharp
private static double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
{
    const double earthRadiusKm = 6371.0;
    var dLat = DegreesToRadians(lat2 - lat1);
    var dLng = DegreesToRadians(lng2 - lng1);

    var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
            Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

    var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
    return earthRadiusKm * c;
}
```

**Accuracy:** ±0.5% error for most distances on Earth

## Future Enhancements
1. Add radius parameter to coordinate query (e.g., "within 10km")
2. Add pagination for partial matches (currently hard-coded to 12)
3. Consider caching distance calculations for frequently queried points
4. Add fuzzy matching for street names (Levenshtein distance)
5. Add lat/lng validation (-90 to 90, -180 to 180)

#### Additional Endpoints
Additional endpoints are exposed by the [Dbx package](https://github.com/tudormobile/dbx/).  
See the [Dbx documentation](https://github.com/tudormobile/dbx/) for details.

---

## New Service Template
Use this section pattern for future tools.

```markdown
## Service: <ServiceName>

### Summary
<1-2 sentence purpose>

### Current Scope and Limitations
- <capability>
- <capability>

### Dependencies
- first dependency
- second dependency
- ...

### Documentation
A link or actual documentation for the app.
```