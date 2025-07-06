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
# Using the provided script
./scripts/generate-dyte-client-kiota.sh dyte-api-spec.yaml

# Or manually
kiota generate \
    --openapi dyte-api-spec.yaml \
    --language csharp \
    --output packages/Flyingdarts.Meetings.Service/Generated \
    --class-name DyteApiClient \
    --namespace-name Flyingdarts.Meetings.Service.Generated \
    --clean-output \
    --serializer SystemTextJson \
    --deserializer SystemTextJson \
    --backing-store
```

### Step 3: Add required NuGet packages

Add the following packages to your project:

```xml
<PackageReference Include="Microsoft.Kiota.Http.HttpClientLibrary" Version="1.1.8" />
<PackageReference Include="Microsoft.Kiota.Serialization.SystemTextJson" Version="1.1.8" />
<PackageReference Include="Microsoft.Kiota.Abstractions" Version="1.1.8" />
```

## Method 2: Using OpenAPI Generator

### Step 1: Install Java (required)

Download and install Java from https://adoptium.net/

### Step 2: Generate the client

```bash
# Using the provided script
./scripts/generate-dyte-client-openapi.ps1 dyte-api-spec.yaml

# Or manually
java -jar openapi-generator-cli.jar generate \
    --input-spec dyte-api-spec.yaml \
    --generator-name csharp-netcore \
    --output packages/Flyingdarts.Meetings.Service/Generated \
    --additional-properties=packageName=Flyingdarts.Meetings.Service.Generated,targetFramework=net8.0
```

## Method 3: Using Visual Studio Connected Services

1. Right-click on your project in Solution Explorer
2. Select "Add" → "Connected Service"
3. Choose "OpenAPI" from the list
4. Select "OpenAPI specification file"
5. Browse to your OpenAPI spec file
6. Configure the namespace and other options
7. Click "Finish"

## Method 4: Using dotnet-openapi (Built-in .NET tool)

```bash
# Install the tool
dotnet tool install --global Microsoft.OpenApi.Kiota


# Generate the client
Depending on your operating system or preference, either run
`pwsh /scripts/generate-dyte-client-kiota.ps1` or
`sh /scripts/generate-dyte-client-kiota.sh`
```

## Integration with Existing Code

After generating the client, you can integrate it with your existing `DyteMeetingService`:

```csharp
using Microsoft.Kiota.Http.HttpClientLibrary;
using Microsoft.Kiota.Serialization.SystemTextJson;
using Flyingdarts.Meetings.Service.Generated;

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
            new SystemTextJsonSerializationWriterFactory(),
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
            return meeting?.Id ?? meetingId;
        }
        catch (ApiException ex) when (ex.ResponseStatusCode == 404)
        {
            // Meeting doesn't exist, create new one
            var createRequest = new CreateMeetingRequest
            {
                Title = $"Meeting {meetingId}",
                Region = "eu-central-1"
            };

            var newMeeting = await _dyteClient.Meetings.PostAsync(createRequest);
            return newMeeting?.Id ?? meetingId;
        }
    }

    public async Task<string> AddParticipantToMeeting(string meetingId)
    {
        var addParticipantRequest = new AddParticipantRequest
        {
            Name = $"Participant {Guid.NewGuid():N}",
            PresetName = _options.DefaultPresetName,
            ClientSpecificId = Guid.NewGuid().ToString()
        };

        var participant = await _dyteClient.Meetings[meetingId].Participants.PostAsync(addParticipantRequest);
        return participant?.Token ?? string.Empty;
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

1. Update your OpenAPI specification file
2. Re-run the generation script
3. Review and test the changes
4. Update your integration code if needed

## Troubleshooting

### Common Issues

1. **Authentication**: Ensure your API credentials are correctly configured
2. **Base URL**: Verify the API base URL is correct
3. **CORS**: Check if CORS policies allow your requests
4. **Rate Limiting**: Implement retry logic for rate-limited requests

### Getting Dyte API Spec

If you don't have the OpenAPI spec, you can:

1. Check Dyte's official documentation
2. Use tools like Swagger Inspector to generate from live API
3. Contact Dyte support for official OpenAPI specification
