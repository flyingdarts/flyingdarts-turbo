# FlyingDarts Turbo Monorepo Development Rules

## Project Structure

- This is a monorepo containing Flutter frontend and .NET backend applications
- Follow the existing folder structure: `apps/` for applications, `packages/` for shared libraries
- Use the established naming conventions for packages and projects

## Code Style & Standards

### Flutter/Dart

- Follow Dart style guide and use `dart format` for code formatting
- Use meaningful variable and function names
- Prefer const constructors when possible
- Use proper null safety practices
- Add documentation comments for public APIs

### .NET/C#

- Follow C# coding conventions
- Use meaningful namespaces that match the project structure
- Implement proper exception handling
- Use async/await patterns for asynchronous operations
- Add XML documentation for public APIs

## Architecture Guidelines

- Keep services and components loosely coupled
- Use dependency injection where appropriate
- Follow SOLID principles
- Implement proper error handling and logging
- Use configuration files for environment-specific settings

## Testing

- Write unit tests for business logic
- Include integration tests for API endpoints
- Maintain good test coverage
- Use descriptive test names that explain the expected behavior

## Git & Version Control

- Use conventional commit messages
- Create feature branches for new development
- Keep commits focused and atomic
- Update CHANGELOG files when making user-facing changes

## Security

- Never commit sensitive information like API keys or passwords
- Use environment variables for configuration
- Validate all user inputs
- Implement proper authentication and authorization

## Performance

- Optimize database queries
- Use caching where appropriate
- Minimize network requests in the frontend
- Profile and optimize performance bottlenecks

## Documentation

- Keep README files up to date
- Document API endpoints and their usage
- Include setup and deployment instructions
- Update documentation when making significant changes
