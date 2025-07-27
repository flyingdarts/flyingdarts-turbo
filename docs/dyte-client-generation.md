# Dyte API Client Generation Guide

This guide explains how to generate a C# client for the Dyte API using OpenAPI specifications.

## Prerequisites

- .NET 8.0 SDK
- OpenAPI specification file for Dyte API (YAML or JSON format)

## Method 1: Using Microsoft Kiota (Recommended)

### Step 1: Install Kiota CLI

```bash
dotnet tool install -g Microsoft.OpenApi.Kiota
```

### Step 2: Generate the client

```bash
# Using the provided script with FIXED specification (recommended)
./scripts/generate-charp-dyte-client.sh

# Or manually with custom parameters
./scripts/generate-charp-dyte-client.sh specs/dyte-api-spec-fixed.yaml packages/Flyingdarts.Meetings.Service/Generated/Dyte Flyingdarts.Meetings.Service.Generated.Dyte DyteApiClient

# Or manually with Kiota directly
kiota generate \
    --openapi specs/dyte-api-spec-fixed.yaml \
    --language csharp \
    --output packages/Flyingdarts.Meetings.Service/Generated/Dyte \
    --class-name DyteApiClient \
    --namespace-name Flyingdarts.Meetings.Service.Generated.Dyte \
    --clean-output \
    --serializer Microsoft.Kiota.Serialization.Json.JsonSerializationWriterFactory \
    --deserializer Microsoft.Kiota.Serialization.Json.JsonParseNodeFactory \
    --backing-store

```

### Step 3: Required NuGet packages

The following packages are already installed and up to date in the project:

```xml
<PackageReference Include="Microsoft.Kiota.Http.HttpClientLibrary" Version="1.1.8" />
<PackageReference Include="Microsoft.Kiota.Serialization.Json" Version="1.1.8" />
<PackageReference Include="Microsoft.Kiota.Abstractions" Version="1.1.8" />
```

✅ **Note**: These packages are already installed and on the latest version, so no additional installation is required.

## API Specification Differences

### Original vs Fixed Specification

The project includes two API specification files:

- **`specs/dyte-api-spec.yaml`** (Original): The complete Dyte API specification with 7,311 lines
- **`specs/dyte-api-spec-fixed.yaml`** (Fixed): A focused subset with 327 lines that resolves schema conflicts

### Key Differences

1. **Scope**: The fixed spec focuses only on the essential meeting and participant endpoints needed for the application
2. **Schema Conflicts**: The original spec had conflicting schema definitions that caused Kiota to generate empty response data classes
3. **Response Structure**: The fixed spec properly defines the response structure with `success` and `data` properties
4. **Type Safety**: The fixed spec ensures proper type definitions for Meeting and Participant objects

### Why Use the Fixed Specification?

The original Dyte API specification contains schema conflicts that cause Kiota to generate empty response data classes. The fixed specification:

- Resolves naming conflicts in schema definitions
- Provides proper type definitions for Meeting and Participant objects
- Ensures the generated client has strongly-typed response objects
- Focuses on the specific endpoints needed for the application

## Method 2: Using OpenAPI Generator

### Step 1: Install Java (required)

Download and install Java from https://adoptium.net/

### Step 2: Generate the client

```bash
# Using the provided script
./scripts/generate-charp-dyte-client-openapi.ps1 specs/dyte-api-spec-fixed.yaml

# Or manually
java -jar openapi-generator-cli.jar generate \
    --input-spec specs/dyte-api-spec-fixed.yaml \
    --generator-name csharp-netcore \
    --output packages/Flyingdarts.Meetings.Service/Generated \
    --additional-properties=packageName=Flyingdarts.Meetings.Service.Generated,targetFramework=net8.0
```

## Method 3: Using Visual Studio Connected Services

1. Right-click on your project in Solution Explorer
2. Select "Add" → "Connected Service"
3. Choose "OpenAPI" from the list
4. Select "OpenAPI specification file"
5. Browse to `specs/dyte-api-spec-fixed.yaml`
6. Configure the namespace and other options
7. Click "Finish"

## Method 4: Using dotnet-openapi (Built-in .NET tool)

```bash
# Install the tool
dotnet tool install --global Microsoft.OpenApi.Kiota

# Generate the client using the provided script
./scripts/generate-charp-dyte-client.sh
```

## Integration with Existing Code

After generating the client, you can integrate it with your existing `DyteMeetingService`:

```csharp
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Serialization.Json;
using Flyingdarts.Meetings.Service.Generated.Dyte;

public class DyteMeetingService : IMeetingService
{
    private readonly DyteApiClient _dyteClient;
    private readonly DyteApiOptions _options;

    public DyteMeetingService(IOptions<DyteApiOptions> options)
    {
        _options = options.Value;

        // Create HTTP client with authentication
        var httpClient = new HttpClient();
        var authHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_options.OrganizationId}:{_options.ApiKey}"));
        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authHeader);

        // Create request adapter
        var requestAdapter = new HttpClientRequestAdapter(
            new JsonSerializationWriterFactory(),
            httpClient: httpClient
        );

        // Create the Kiota client
        _dyteClient = new DyteApiClient(requestAdapter);
    }

    public async Task<string> GetOrCreateMeeting(string meetingId)
    {
        try
        {
            // Try to get existing meeting
            var meeting = await _dyteClient.Meetings[meetingId].GetAsync();
            return meeting?.Data?.Id ?? meetingId;
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == 404)
        {
            // Meeting doesn't exist, create new one
            var createRequest = new CreateMeetingRequest
            {
                Title = $"Meeting {meetingId}",
                PreferredRegion = "eu-central-1"
            };

            var newMeeting = await _dyteClient.Meetings.PostAsync(createRequest);
            return newMeeting?.Data?.Id ?? meetingId;
        }
    }

    public async Task<string> AddParticipantToMeeting(string meetingId)
    {
        var addParticipantRequest = new AddParticipantRequest
        {
            Name = $"Participant {Guid.NewGuid():N}",
            PresetName = _options.DefaultPresetName,
            CustomParticipantId = Guid.NewGuid().ToString()
        };

        var participant = await _dyteClient.Meetings[meetingId].Participants.PostAsync(addParticipantRequest);
        return participant?.Data?.Token ?? string.Empty;
    }
}
```

## Configuration Updates

Update your `ServiceCollectionExtensions.cs`:

```csharp
public static IServiceCollection AddDyteMeetingService(this IServiceCollection services, IConfiguration configuration)
{
    // Configure Dyte API options
    services.Configure<DyteApiOptions>(configuration.GetSection(DyteApiOptions.SectionName));

    // Register your service (Kiota client is created within the service)
    services.AddScoped<IMeetingService, DyteMeetingService>();

    return services;
}
```

## Benefits of Kiota Generated Client

1. **Type Safety**: All API calls are strongly typed with full IntelliSense support
2. **Microsoft Backed**: Official Microsoft tool with active development
3. **Performance**: Optimized for .NET with minimal overhead
4. **Modern .NET**: Built for .NET 6+ with modern patterns
5. **Flexible Authentication**: Easy to integrate with various auth schemes
6. **Request/Response Interception**: Built-in middleware support
7. **Backing Store**: Optional change tracking for entities
8. **Cross-Platform**: Works on Windows, macOS, and Linux
9. **Extensible**: Plugin architecture for custom serializers
10. **Validation**: Built-in request/response validation

## Updating the Client

When the Dyte API changes:

1. Update your OpenAPI specification file (preferably the fixed version)
2. Re-run the generation script: `./scripts/generate-charp-dyte-client.sh`
3. Review and test the changes
4. Update your integration code if needed

## Troubleshooting

### Common Issues

1. **Authentication**: Ensure your API credentials are correctly configured
2. **Base URL**: Verify the API base URL is correct
3. **CORS**: Check if CORS policies allow your requests
4. **Rate Limiting**: Implement retry logic for rate-limited requests
5. **Empty Response Classes**: Use the fixed specification to avoid schema conflicts

### Getting Dyte API Spec

If you don't have the OpenAPI spec, you can:

1. Check Dyte's official documentation
2. Use tools like Swagger Inspector to generate from live API
3. Contact Dyte support for official OpenAPI specification
4. Use the provided fixed specification as a starting point
