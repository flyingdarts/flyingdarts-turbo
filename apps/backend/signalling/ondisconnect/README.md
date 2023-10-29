# Flyingdarts.Backend.Signalling.OnDisconnect
A C# .NET6 top-level lambda function.
Generated with Flyingdarts.Utility.Templates

## Contents

### Contains
- an implementation of CQRS
- an implementation of the command handling aspect of the CQRS pattern using the IMediatR library
- an implementation of the command validation aspect of the CQRS pattern using the FluentValidation library

### Files
Flyingdarts.Backend.Signalling.OnDisconnect
    .github
        workflows
            build-and-publish-package.yml
    Function.cs
    InnerHandler.cs
    ServiceFactory.cs
    CQRS
        OnDisconnectCommand.cs
        OnDisconnectCommandHandler.cs
        OnDisconnectCommandValidator.cs
    LICENSE.md
    README.md