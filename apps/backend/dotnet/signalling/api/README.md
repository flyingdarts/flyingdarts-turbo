# Flyingdarts Backend Signalling API

This is a consolidated WebSocket API that handles all WebSocket connections for the Flyingdarts application. It replaces the previous three separate lambda functions (onconnect, ondefault, ondisconnect) with a single, unified API.

## Overview

The Signalling API consolidates the following WebSocket routes into a single lambda function:

- `$connect` - Handles new WebSocket connections
- `$default` - Handles default message routing and broadcasting
- `$disconnect` - Handles WebSocket disconnections

## Architecture

The API follows the same pattern as the X01 API, using:

- **MediatR** for command/query handling
- **CQRS pattern** with separate command handlers for each route
- **Dependency injection** for service management
- **AWS Lambda** with API Gateway WebSocket integration

## Routes

### $connect
- **Command**: `OnConnectCommand`
- **Handler**: `OnConnectCommandHandler`
- **Purpose**: Establishes new WebSocket connections, creates/updates user records, and ensures meeting room availability

### $default
- **Command**: `OnDefaultCommand`
- **Handler**: `OnDefaultCommandHandler`
- **Purpose**: Handles message broadcasting to all connected clients

### $disconnect
- **Command**: `OnDisconnectCommand`
- **Handler**: `OnDisconnectCommandHandler`
- **Purpose**: Cleans up connection records when clients disconnect

## Dependencies

- `Flyingdarts.Lambda.Core` - Core lambda infrastructure
- `Flyingdarts.Connection.Services` - Connection management
- `Flyingdarts.DynamoDb.Service` - Database operations
- `Flyingdarts.Meetings.Service` - Meeting room management
- `Flyingdarts.Metadata.Services` - Metadata services
- `Flyingdarts.NotifyRooms.Service` - Room notification services
- `Flyingdarts.Persistence` - Persistence layer

## Environment Variables

- `TableName` - DynamoDB table name for signalling data
- `WebSocketApiUrl` - WebSocket API Gateway URL (set by CDK)

## Deployment

This API should be deployed together with the games websocket api ok