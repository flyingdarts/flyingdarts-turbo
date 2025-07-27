# Internal Package Dependencies

This document lists all internal package dependencies between the Flyingdarts .NET packages.

## Package Dependency Matrix

| Package | Internal Dependencies | External Dependencies |
|---------|----------------------|----------------------|
| **Flyingdarts.Lambda.Core** | `flyingdarts.core` | AWS SDK, MediatR, FluentValidation, Microsoft Extensions |
| **Flyingdarts.Core** | None | AWS SDK, Microsoft Extensions |
| **Flyingdarts.Connection.Services** | `flyingdarts.persistence`, `flyingdarts.dynamodb.service` | AWS SDK, Microsoft Extensions |
| **Flyingdarts.Persistence** | None | AWS SDK, Microsoft Extensions |
| **Flyingdarts.DynamoDb.Service** | `flyingdarts.persistence` | AWS SDK |
| **Flyingdarts.Meetings.Service** | None | AWS SDK, Microsoft Kiota |
| **Flyingdarts.NotifyRooms.Service** | `flyingdarts.core` | AWS SDK, Microsoft Extensions |
| **Flyingdarts.Metadata.Services** | `flyingdarts.persistence`, `flyingdarts.dynamodb.service` | AWS SDK, Microsoft Extensions |

## Dependency Graph

```
Flyingdarts.Lambda.Core
├── Flyingdarts.Core

Flyingdarts.Connection.Services
├── Flyingdarts.DynamoDb.Service
│   └── Flyingdarts.Persistence
└── Flyingdarts.Persistence

Flyingdarts.DynamoDb.Service
└── Flyingdarts.Persistence

Flyingdarts.NotifyRooms.Service
└── Flyingdarts.Core

Flyingdarts.Metadata.Services
├── Flyingdarts.DynamoDb.Service
│   └── Flyingdarts.Persistence
└── Flyingdarts.Persistence
```

## Package.json Dependencies

All packages now have their internal dependencies properly documented in their respective `package.json` files:

### Flyingdarts.Lambda.Core
```json
{
  "dependencies": {
    "flyingdarts.core": "*"
  }
}
```

### Flyingdarts.Connection.Services
```json
{
  "dependencies": {
    "flyingdarts.persistence": "*",
    "flyingdarts.dynamodb.service": "*"
  }
}
```

### Flyingdarts.DynamoDb.Service
```json
{
  "dependencies": {
    "flyingdarts.persistence": "*"
  }
}
```

### Flyingdarts.NotifyRooms.Service
```json
{
  "dependencies": {
    "flyingdarts.core": "*"
  }
}
```

### Flyingdarts.Metadata.Services
```json
{
  "dependencies": {
    "flyingdarts.persistence": "*",
    "flyingdarts.dynamodb.service": "*"
  }
}
```

## Notes

- All internal dependencies use `"*"` version to allow for flexible versioning within the monorepo
- External dependencies are managed through the `.csproj` files with specific version numbers
- The dependency graph shows that `Flyingdarts.Persistence` is the most foundational package, with no internal dependencies
- `Flyingdarts.Core` is also foundational, with no internal dependencies
- `Flyingdarts.Lambda.Core` is the newest package, providing infrastructure for Lambda functions 