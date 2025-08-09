# Flying Darts Meetings Service

## Overview

The Flying Darts Meetings Service is a .NET 8 library that provides comprehensive video conferencing functionality for the Flying Darts gaming platform. This service integrates with the Dyte API to manage video meetings, enabling players to create, join, and participate in real-time video calls during their darts games.

The service is built using modern .NET patterns with dependency injection, providing a clean abstraction layer over the Dyte API. It includes auto-generated client code using Microsoft Kiota for type-safe API interactions and supports both meeting management and participant handling.

## Features

- **Meeting Management**: Create, retrieve, and search for video meetings
- **Participant Management**: Add participants to meetings with proper authentication
- **Dyte API Integration**: Full integration with Dyte's video conferencing platform
- **Type-Safe API Client**: Auto-generated client using Microsoft Kiota
- **Configuration Management**: Flexible configuration for Dyte API credentials
- **Error Handling**: Comprehensive error handling and logging
- **Dependency Injection**: Built-in support for DI container integration
- **Async Operations**: Full async/await support for all operations

## Prerequisites

- **.NET 8 SDK**: Required for building and using the library
- **Dyte Account**: Active Dyte organization account with API credentials
- **Dyte API Access**: Valid organization ID and API key from Dyte
- **HTTP Client**: HttpClient for making API requests (provided by DI)

## Installation

1. **Clone the repository** (if not already done):
   ```bash
   git clone https://github.com/flyingdarts/flyingdarts.git
   cd flyingdarts
   ```

2. **Navigate to the meetings service**:
   ```bash
   cd packages/backend/dotnet/Flyingdarts.Meetings.Service/Flyingdarts.Meetings.Service
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Build the project**:
   ```bash
   dotnet build
   ```

## Usage

### Basic Setup

1. **Add the service to your DI container**:
   ```csharp
   services.AddScoped<IMeetingService, DyteMeetingService>();
   services.AddScoped<IDyteApiClientWrapper, DyteApiClientWrapper>();
   services.AddHttpClient<DyteApiClientFactory>();
   ```

2. **Configure Dyte API settings**:
   ```csharp
   services.Configure<DyteApiOptions>(configuration.GetSection("DyteApi"));
   ```

3. **Set environment variables**:
   ```bash
   export DyteOrganizationId="your-organization-id"
   export DyteApiKey="your-api-key"
   ```

### Creating a Meeting

```csharp
public class MeetingController
{
    private readonly IMeetingService _meetingService;

    public MeetingController(IMeetingService meetingService)
    {
        _meetingService = meetingService;
    }

    public async Task<IActionResult> CreateMeeting(string roomName)
    {
        var meeting = await _meetingService.CreateAsync(roomName, CancellationToken.None);
        return Ok(meeting);
    }
}
```

### Adding Participants

```csharp
public async Task<string> JoinMeeting(string meetingId, string participantName)
{
    var request = new JoinMeetingRequest
    {
        MeetingId = meetingId,
        ParticipantName = participantName
    };

    var token = await _meetingService.AddParticipantAsync(request, CancellationToken.None);
    return token;
}
```

## API Reference

### Core Interfaces

#### IMeetingService
Main service interface for meeting operations.

```csharp
public interface IMeetingService
{
    Task<Meeting?> CreateAsync(string name, CancellationToken cancellationToken);
    Task<Meeting?> GetByIdAsync(Guid meetingId, CancellationToken cancellationToken);
    Task<Meeting?> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<IEnumerable<Meeting>?> GetAllAsync(CancellationToken cancellationToken);
    Task<string?> AddParticipantAsync(JoinMeetingRequest request, CancellationToken cancellationToken);
}
```

#### IDyteApiClientWrapper
Wrapper interface for Dyte API client operations.

```csharp
public interface IDyteApiClientWrapper
{
    Task<MeetingsPostResponse> CreateMeetingAsync(CreateMeetingRequest request, CancellationToken cancellationToken);
    Task<WithMeeting_GetResponse> GetMeetingByIdAsync(string meetingId, CancellationToken cancellationToken);
    Task<MeetingsGetResponse> SearchMeetingsAsync(string searchQuery, CancellationToken cancellationToken);
    Task<MeetingsGetResponse> GetAllMeetingsAsync(CancellationToken cancellationToken);
    Task<ParticipantsPostResponse> AddParticipantAsync(string meetingId, AddParticipantRequest request, CancellationToken cancellationToken);
}
```

### Data Models

#### Meeting
Represents a Dyte meeting with all its properties.

#### JoinMeetingRequest
Request model for joining a meeting.

```csharp
public class JoinMeetingRequest
{
    public string MeetingId { get; set; } = string.Empty;
    public string ParticipantName { get; set; } = string.Empty;
}
```

#### DyteApiOptions
Configuration options for Dyte API integration.

```csharp
public class DyteApiOptions
{
    public const string SectionName = "DyteApi";
    
    public string BaseUrl { get; set; } = "https://api.dyte.io/v2";
    public string OrganizationId { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string DefaultPresetName { get; set; } = "default";
}
```

### Service Methods

#### CreateAsync(string name, CancellationToken cancellationToken)
Creates a new meeting with the specified name.

- **Parameters**:
  - `name` (string): Base name for the meeting
  - `cancellationToken` (CancellationToken): Cancellation token
- **Returns**: Task<Meeting?> - The created meeting or null if failed
- **Throws**: ArgumentNullException if name is null or empty

#### GetByIdAsync(Guid meetingId, CancellationToken cancellationToken)
Retrieves a meeting by its unique identifier.

- **Parameters**:
  - `meetingId` (Guid): The meeting ID
  - `cancellationToken` (CancellationToken): Cancellation token
- **Returns**: Task<Meeting?> - The meeting or null if not found

#### GetByNameAsync(string name, CancellationToken cancellationToken)
Searches for a meeting by its formatted name.

- **Parameters**:
  - `name` (string): The base name to search for
  - `cancellationToken` (CancellationToken): Cancellation token
- **Returns**: Task<Meeting?> - The meeting or null if not found

#### GetAllAsync(CancellationToken cancellationToken)
Retrieves all meetings for the organization.

- **Parameters**:
  - `cancellationToken` (CancellationToken): Cancellation token
- **Returns**: Task<IEnumerable<Meeting>?> - Collection of meetings

#### AddParticipantAsync(JoinMeetingRequest request, CancellationToken cancellationToken)
Adds a participant to a meeting and returns the participant token.

- **Parameters**:
  - `request` (JoinMeetingRequest): The join request details
  - `cancellationToken` (CancellationToken): Cancellation token
- **Returns**: Task<string?> - The participant token or null if failed

## Configuration

### Environment Variables
The service requires the following environment variables:

- **DyteOrganizationId**: Your Dyte organization ID
- **DyteApiKey**: Your Dyte API key

### Configuration File
You can also configure the service using appsettings.json:

```json
{
  "DyteApi": {
    "BaseUrl": "https://api.dyte.io/v2",
    "OrganizationId": "your-organization-id",
    "ApiKey": "your-api-key",
    "DefaultPresetName": "group_call_participant"
  }
}
```

### Default Settings
- **Preferred Region**: EU Central 1 (optimized for European users)
- **Meeting Name Template**: "{0} Flyingdarts Room"
- **Default Preset**: "group_call_participant"

## Development

### Project Structure
```
Flyingdarts.Meetings.Service/
├── Configuration/                 # Configuration classes
│   └── DyteApiOptions.cs
├── Factories/                     # Factory classes
│   └── DyteApiClientFactory.cs
├── Generated/                     # Auto-generated Dyte API client
│   └── Dyte/
│       ├── Models/               # Dyte API models
│       ├── Meetings/             # Meeting-related operations
│       └── DyteApiClient.cs      # Main API client
├── Services/                      # Service implementations
│   ├── IMeetingService.cs        # Main service interface
│   ├── IDyteApiClientWrapper.cs  # API client wrapper interface
│   ├── DyteApiClientWrapper.cs   # API client wrapper implementation
│   └── DyteMeetingService/       # Main service implementation
│       ├── DyteMeetingService.cs
│       ├── Requests/             # Request models
│       └── Responses/            # Response models
├── Dtos/                         # Data transfer objects
├── GlobalUsings.cs               # Global using directives
├── dyte-api-spec-fixed.yaml      # Dyte API specification
└── Flyingdarts.Meetings.Service.csproj
```

### Architecture Patterns
- **Dependency Injection**: Service lifecycle management
- **Factory Pattern**: Client creation and configuration
- **Wrapper Pattern**: Abstraction over external API
- **Configuration Pattern**: Options-based configuration
- **Repository Pattern**: Data access abstraction

### Code Generation
The Dyte API client is auto-generated using Microsoft Kiota from the OpenAPI specification:

```bash
# Regenerate the client (if needed)
kiota generate --openapi dyte-api-spec-fixed.yaml --output Generated/Dyte
```

### Testing
Run unit tests to ensure code quality:
```bash
dotnet test
```

### Code Quality
- Follow C# coding conventions
- Use XML documentation for public APIs
- Implement proper error handling
- Add logging for debugging
- Use nullable reference types

## Dependencies

### External Dependencies
- **AWSSDK.SimpleSystemsManagement** (4.0.2.4): AWS Systems Manager integration
- **Microsoft.Extensions.Configuration** (10.0.0-preview.6.25358.103): Configuration management
- **Microsoft.Kiota.Abstractions** (1.19.0): Kiota abstractions
- **Microsoft.Kiota.Http.HttpClientLibrary** (1.19.0): HTTP client library
- **Microsoft.Kiota.Serialization.Json** (1.19.0): JSON serialization
- **Microsoft.Kiota.Serialization.Text** (1.19.0): Text serialization

### Internal Dependencies
This is a standalone service library with no internal dependencies.

## Related Projects

### Backend Services
- **[Friends API](../../../apps/backend/dotnet/friends/)**: Friend management and social features
- **[Auth API](../../../apps/backend/dotnet/auth/)**: Authentication and authorization
- **[Games API](../../../apps/backend/dotnet/games/)**: Game management and scoring

### Frontend Applications
- **[Flutter Mobile App](../../../../frontend/flutter/flyingdarts_mobile/)**: Mobile application
- **[Angular Web App](../../../../frontend/angular/fd-app/)**: Web application

### Shared Packages
- **[Core Package](../../../Flyingdarts.Core/)**: Shared business logic
- **[Persistence Package](../../../Flyingdarts.Persistence/)**: Data access layer
- **[DynamoDB Service](../../../Flyingdarts.DynamoDb.Service/)**: Database operations

## Version History

- **v0.0.9** (2025-07-26): Implemented friends feature
- **v0.0.8** (2025-07-19): Pipeline up and running
- **v0.0.7** (2025-07-14): Working flutter pipeline / run app on sim
- **v0.0.6** (2025-07-10): Add flutter workspace at root
- **v0.0.5** (2025-07-10): Re-organize abit
- **v0.0.4** (2025-07-08): Make ci
- **v0.0.3** (2025-07-08): Make & restore solution

## Contributing

1. Follow the established coding standards
2. Add comprehensive tests for new features
3. Update documentation for API changes
4. Ensure all builds pass before submitting PRs
5. Update the Dyte API specification if needed
6. Regenerate the client code after API changes

## License

This project is part of the Flying Darts platform and is subject to the project's licensing terms.
