# Flyingdarts.Backend.Games.X01.JoinQueue
A C# .NET6 top-level lambda function.
Generated with Flyingdarts.Utility.Templates

## Contents

### Contains
- an implementation of CQRS
- an implementation of the command handling aspect of the CQRS pattern using the IMediatR library
- an implementation of the command validation aspect of the CQRS pattern using the FluentValidation library

### Files
Flyingdarts.Backend.Games.X01.JoinQueue
    .github
        workflows
            build-and-publish-package.yml
    Function.cs
    InnerHandler.cs
    ServiceFactory.cs
    CQRS
        JoinX01QueueCommand.cs
        JoinX01QueueCommandHandler.cs
        JoinX01QueueCommandValidator.cs
    LICENSE.md
    README.md