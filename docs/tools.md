# Community Tools
Reference documentation for utilities in the `tools` folder.

## Purpose
This document provides tool-by-tool notes for development and validation utilities used by Community.

## Tool Index
| Tool | Folder | Status | Primary Purpose |
| --- | --- | --- | --- |
| nys_sam_addresses | `tools/nys_sam_addresses` | Active | Extracts address data from NYS Street and Address Maintenance (SAM) database. |

---

## Tool: nys_sam_addresses

### Summary
Console utility that queries NYS SAM database and downloads address information to local JSON and GeoJSON files.

### Current Scope
- Downloads street name, number, location information from SAM database to GeoJSON file.
- Extracts and creates JSON files for address points and unique street names.

### Tech/Dependencies
- Runtime: .NET `net10.0`
- Package: `System.Text/Json`
- Platform expectation: none.

### Run
From repository root:

```powershell
dotnet run --project .\tools\nys_sam_addresses\nys_sam_addresses.csproj
```

### Output
- Progress information, including record counts.
- Status information, names of files created.

### Notes
- Query is currently setup to download a single town (currently set to 'Clifton Park').
- Data is retrieved in 1,000 record chunks.

---

## New Tool Template
Use this section pattern for future tools.

```markdown
## Tool: <ToolName>

### Summary
<1-2 sentence purpose>

### Current Scope
- <capability>
- <capability>

### Tech/Dependencies
- Runtime: <framework/runtime>
- Package(s): <if any>
- Platform expectation: <os/environment>

### Run
~~~powershell
dotnet run --project .\tools\<ToolFolder>\<ToolProject>.csproj
~~~

### Output
- <key output 1>
- <key output 2>

### Notes
- <limitations, caveats, or naming notes>
```