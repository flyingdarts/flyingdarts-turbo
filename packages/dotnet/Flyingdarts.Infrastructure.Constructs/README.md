## Table of Contents

- [AmazonStack.cs](#amazonstackcs)
- [AmplifyConstruct.cs](#amplifyconstructcs)
- [AuthConstruct.cs](#authconstructcs)
- [BackendConstruct.cs](#backendconstructcs)

## AmazonStack.cs
The AmazonStack class represents an AWS CloudFormation stack for an Amazon application. It extends the Stack class.

### Constructor
```csharp
public AmazonStack(Construct scope, IStackProps props, string[] repositories) : base(scope, "Flyingdarts-Stack", props)
```

The constructor creates a new instance of the AmazonStack class. It takes the following parameters:

- `scope`: The parent construct of the stack.
- `props`: Stack properties.
- `repositories`: An array of repository names.

## AmplifyConstruct.cs
The AmplifyConstruct class represents an AWS Amplify construct for the frontend application.

### Constructor
```csharp
public AmplifyConstruct(Construct scope, string id) : base(scope, id)
```
The constructor creates a new instance of the AmplifyConstruct class. It takes the following parameters:

- `scope`: The parent construct of the Amplify construct.
- `id`: The ID of the construct.

The constructor sets up the Amplify application, CodeCommit repository, branches, and domain for the frontend application.

## AuthConstruct.cs
The AuthConstruct class represents an AWS Cognito construct for authentication.

### Constructor
```csharp
public AuthConstruct(Construct scope, string id, string[] repositories) : base(scope, id)
```
The constructor creates a new instance of the AuthConstruct class. It takes the following parameters:

- `scope`: The parent construct of the Auth construct.
- `id`: The ID of the construct.
- `repositories`: An array of repository names.

The constructor sets up the OpenID Connect provider for authentication and creates roles for Github Actions to deploy functions to AWS Lambda.

## BackendConstruct.cs
The BackendConstruct class represents a backend construct for the Amazon application.

### Constructor
```csharp
public BackendConstruct(Construct scope, string id, string[] repositories) : base(scope, id)
```
The constructor creates a new instance of the BackendConstruct class. It takes the following parameters:

- `scope`: The parent construct of the backend construct.
- `id`: The ID of the construct.
- `repositories`: An array of repository names.

The constructor sets up the signaling table, Lambda functions, WebSocket API, and application table for the backend.

### Usage
To use the AmazonStack class, create a new instance of it and pass the required parameters.

Example:

```csharp
var stack = new AmazonStack(this, "MyAmazonStack", props, repositories);
```

Make sure to replace props with the actual stack properties and repositories with the array of repository names.