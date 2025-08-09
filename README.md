# Flyingdarts monorepo

## Introduction

Flyingdarts is the result of 4–5 years of dedicated work. Over this time, it has matured into a solid and trustworthy platform, grounded in my experience as a full‑stack developer.

The platform is open source and built for the community. Anyone can suggest features and help implement them. Flyingdarts is community‑driven.

## Project Overview

### Frontend

- **Angular app** (`apps/frontend/angular/fd-app`): The main platform where players play darts against each other with real‑time gameplay and integrated video conferencing.
- **Flutter app** (`apps/frontend/flutter/flyingdarts_mobile`): A companion app that lets players input their scores using native speech‑to‑text on Android and iOS while they are in an active game.

### Backend

- Built as small, independently deployable microservices on AWS for horizontal scalability. Primarily implemented in C#; a Rust‑based authorizer is under active development.
- WebSocket microservices (fronted by Amazon API Gateway WebSocket):
  - `Signalling` (Lambda): Reusable connection orchestration and signalling logic.
  - `Backend Api` (Lambda): Application‑specific WebSocket routes and business logic.
  - These are separate microservices that can scale and deploy independently. The API Gateway and Lambda integrations are provisioned via the CDK constructs.
- REST microservice (fronted by Amazon API Gateway REST):
  - `Friends API` (Lambda): Social features (friend relationships) for the platform.
- Authorization: Both the WebSocket and Friends REST microservices are protected by a C# Lambda authorizer (`apps/backend/dotnet/auth`).

### Authentication

- Service provider: Authress handles identity and authentication for the platform.
- Flutter integration: A dedicated package simplifies integrating Authress into the Flutter widget tree — see `packages/frontend/flutter/authress/login`.

### Infrastructure as Code

- All backend and frontend hosting for development, staging, and production is defined with AWS CDK stacks, leveraging custom constructs from `packages/tools/dotnet/Flyingdarts.CDK.Constructs`.
- Supporting stacks include:
  - An OIDC authentication stack enabling the GitHub pipeline to assume an AWS role and deploy to AWS Lambda and Amazon S3.
  - A domain/certificate stack that references the Flyingdarts Route 53 HostedZone and ACM certificate(s) used by the frontends and APIs.

## TODOs

- Figure out proper usage of Beachball for versioning/releasing and integrate it into the CI/CD pipeline.
- Set up consistent linting across projects/frameworks/languages (e.g., Angular/TypeScript, Flutter/Dart, .NET/C#, Rust).
- Set up CI to run tests across all projects/frameworks/languages (Angular/TypeScript, Flutter/Dart, .NET/C#, Rust).
- Publish the Flutter app to the app stores (requires developer accounts and release process setup).
- iOS publishing: Create Xcode schemes/build configurations for Flutter `--flavor` builds (e.g., `dev`, `acc` (staging), `prod`) and configure Fastlane lanes to build/sign/distribute per flavor:
  - dev: do not publish (local/debug/internal only)
  - staging (acc): publish to TestFlight
  - prod: publish to App Store

## Contributing

Stoked you're here! This is a friendly, low‑pressure space:

- Be kind and constructive.
- Prefer small, focused PRs.
- If you’re unsure, please hop into our [Discord](https://discord.gg/BqQxwfdDhC) and start a chat/thread about the feature or issue you want to solve.
- Not sure where to start? Check issues labeled “good first issue”.

### Quick start

1. Fork and clone your fork.
2. Create a branch: `git checkout -b my-feature`.
3. Install deps and run the app(s) you’re touching; see the app READMEs:
   - Angular: `apps/frontend/angular/fd-app/README.md`
   - Flutter: `apps/frontend/flutter/flyingdarts_mobile/README.md`
   - Backend (C#): see service READMEs under `apps/backend/dotnet/`
4. Run tests/linters before pushing.

### PR checklist

- Clear title and description (what + why).
- Screenshots for UI changes.
- Tests or reasoning for behavior changes.
- No linter errors; CI is green.
- Docs/README updated if needed.

### Communication

- Join the community chat on our [Discord server](https://discord.gg/BqQxwfdDhC).
- For bigger ideas, start a thread on Discord first; we can turn it into a scoped issue together.

### Style

- Match the existing code style and formatter/linter configs.
- Keep functions small and names clear.

### Code of Conduct

We follow the [Contributor Covenant](https://www.contributor-covenant.org/version/2/1/code_of_conduct/). Be excellent to each other.

### License

By contributing, you agree your changes are licensed under this repository’s license.
