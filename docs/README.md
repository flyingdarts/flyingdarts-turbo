# Flying Darts Turbo Documentation

Welcome to the comprehensive documentation for the Flying Darts Turbo monorepo. This documentation provides detailed information about all applications, packages, and tools in the Flying Darts platform.

## 🎯 Overview

Flying Darts Turbo is a comprehensive darts gaming platform built as a monorepo containing multiple applications, services, and packages. The platform provides real-time multiplayer darts gaming with social features, voice communication, and cross-platform support.

## 📁 Documentation Structure

```
docs/
├── README.md                    # This main documentation index
├── apps/                        # Application documentation
│   ├── README.md               # Applications overview
│   ├── backend/                # Backend applications
│   │   ├── dotnet/            # .NET backend apps
│   │   │   ├── api/           # X01 Game API
│   │   │   ├── auth/          # Authentication service
│   │   │   ├── friends/       # Friends management API
│   │   │   └── signalling/    # Real-time communication
│   │   └── rust/              # Rust backend apps
│   │       └── authorizer/    # Custom authorization
│   ├── frontend/               # Frontend applications
│   │   ├── angular/           # Angular web app
│   │   │   └── fd-app/        # Flying Darts Angular App
│   │   └── flutter/           # Flutter mobile app
│   │       └── flyingdarts_mobile/ # Mobile application
│   └── tools/                  # Development tools
│       └── dotnet/            # .NET tools
│           └── cdk/           # CDK Infrastructure
├── packages/                    # Package documentation
│   ├── README.md              # Packages overview
│   ├── backend/               # Backend packages
│   │   ├── dotnet/           # .NET backend packages
│   │   └── rust/             # Rust backend packages
│   ├── frontend/              # Frontend packages
│   │   └── flutter/          # Flutter frontend packages
│   └── tools/                 # Development tools
│       ├── config/           # Configuration package
│       └── dotnet/           # .NET tools
└── agent/                      # Documentation agent
    ├── README.md             # Agent overview
    ├── documentation-agent-specification.md # Agent specification
    ├── docs-agent.config.json # Agent configuration
    └── templates/            # Documentation templates
```

## 🚀 Quick Start

### Getting Started

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-org/flyingdarts-turbo.git
   cd flyingdarts-turbo
   ```

2. **Install Dependencies**:
   ```bash
   # Install .NET dependencies
   dotnet restore

   # Install Flutter dependencies
   flutter pub get

   # Install Node.js dependencies
   npm install
   ```

3. **Configure Environment**:
   ```bash
   # Copy environment files
   cp apps/backend/dotnet/api/appsettings.example.json apps/backend/dotnet/api/appsettings.json
   cp apps/frontend/angular/fd-app/src/environments/environment.example.ts apps/frontend/angular/fd-app/src/environments/environment.ts
   ```

4. **Start Development**:
   ```bash
   # Start backend services
   cd apps/backend/dotnet/api && dotnet run
   cd apps/backend/dotnet/auth && dotnet run

   # Start frontend applications
   cd apps/frontend/angular/fd-app && npm start
   cd apps/frontend/flutter/flyingdarts_mobile && flutter run
   ```

## 🏗️ Architecture Overview

### System Architecture

The Flying Darts Turbo platform follows a microservices architecture with the following components:

#### Backend Services
- **X01 Game API**: Core game logic and scoring
- **Authentication Service**: User authentication and authorization
- **Friends API**: Social features and friend management
- **Signalling Service**: Real-time WebSocket communication
- **Rust Authorizer**: Custom API Gateway authorization

#### Frontend Applications
- **Angular Web App**: Web-based darts application
- **Flutter Mobile App**: Cross-platform mobile application

#### Infrastructure
- **AWS CDK**: Infrastructure as Code
- **Lambda Functions**: Serverless backend services
- **API Gateway**: REST and WebSocket APIs
- **DynamoDB**: NoSQL data storage
- **CloudWatch**: Monitoring and logging

### Technology Stack

#### Backend Technologies
- **.NET 8**: Primary backend framework
- **Rust**: High-performance authorization
- **AWS Lambda**: Serverless computing
- **DynamoDB**: NoSQL database
- **WebSocket**: Real-time communication

#### Frontend Technologies
- **Angular 18**: Web application framework
- **Flutter**: Cross-platform mobile framework
- **TypeScript**: Type-safe JavaScript
- **Dart**: Flutter programming language

#### DevOps & Tools
- **AWS CDK**: Infrastructure as Code
- **GitHub Actions**: CI/CD pipelines
- **Docker**: Containerization
- **Jest**: Testing framework

## 📚 Documentation Sections

### Applications Documentation

#### Backend Applications
- **[X01 Game API](apps/backend/dotnet/api/README.md)**: Real-time multiplayer darts game API
- **[Authentication Service](apps/backend/dotnet/auth/README.md)**: JWT token validation and authorization
- **[Friends API](apps/backend/dotnet/friends/README.md)**: Social features and friend management
- **[Signalling Service](apps/backend/dotnet/signalling/README.md)**: WebSocket-based real-time communication
- **[Rust Authorizer](apps/backend/rust/authorizer/README.md)**: High-performance custom authorization

#### Frontend Applications
- **[Angular Web App](apps/frontend/angular/fd-app/README.md)**: Web-based darts application with real-time features
- **[Flutter Mobile App](apps/frontend/flutter/flyingdarts_mobile/README.md)**: Cross-platform mobile application

#### Development Tools
- **[CDK Infrastructure](apps/tools/dotnet/cdk/README.md)**: AWS infrastructure as code

### Packages Documentation

#### Backend Packages
- **[Core Library](packages/backend/dotnet/Flyingdarts.Core/)**: Shared models and utilities
- **[Lambda Core](packages/backend/dotnet/Flyingdarts.Lambda.Core/)**: AWS Lambda infrastructure
- **[Persistence Layer](packages/backend/dotnet/Flyingdarts.Persistence/)**: Data access patterns
- **[Connection Services](packages/backend/dotnet/Flyingdarts.Connection.Services/)**: WebSocket management
- **[DynamoDB Service](packages/backend/dotnet/Flyingdarts.DynamoDb.Service/)**: Database operations
- **[Meetings Service](packages/backend/dotnet/Flyingdarts.Meetings.Service/)**: Meeting management
- **[Metadata Services](packages/backend/dotnet/Flyingdarts.Metadata.Services/)**: Configuration management
- **[Notification Service](packages/backend/dotnet/Flyingdarts.NotifyRooms.Service/)**: Real-time notifications
- **[Rust Auth](packages/backend/rust/auth/)**: Authentication utilities

#### Frontend Packages
- **[API SDK](packages/frontend/flutter/api/)**: Backend API integration
- **[Authress Integration](packages/frontend/flutter/authress/)**: Authentication components
- **[Core Package](packages/frontend/flutter/core/)**: Shared Flutter utilities
- **[Feature Packages](packages/frontend/flutter/features/)**: Modular features
- **[Shared Components](packages/frontend/flutter/shared/)**: Common UI components

#### Tools Packages
- **[Configuration Package](packages/tools/config/)**: Shared configuration
- **[CDK Constructs](packages/tools/dotnet/Flyingdarts.CDK.Constructs/)**: Infrastructure components

## 🔧 Development Guidelines

### Code Standards

#### .NET Development
- Follow C# coding conventions
- Use XML documentation for public APIs
- Implement comprehensive unit tests
- Follow SOLID principles

#### Flutter Development
- Follow Dart style guide
- Use BLoC pattern for state management
- Implement widget tests
- Follow Material Design guidelines

#### Rust Development
- Follow Rust coding conventions
- Use comprehensive error handling
- Implement unit and integration tests
- Follow Rust best practices

### Testing Strategy

#### Unit Testing
- High test coverage (>80%)
- Isolated test execution
- Mock external dependencies
- Fast test execution

#### Integration Testing
- End-to-end workflow testing
- Cross-service integration
- Performance testing
- Security testing

#### UI Testing
- Component testing
- User journey testing
- Cross-browser compatibility
- Mobile device testing

### Documentation Standards

#### Code Documentation
- Inline comments for complex logic
- API documentation for public interfaces
- Architecture decision records (ADRs)
- Code examples and usage patterns

#### Project Documentation
- Comprehensive README files
- Setup and installation guides
- API reference documentation
- Troubleshooting guides

## 🚀 Deployment

### Environment Management

#### Development Environment
- Local development setup
- Docker containerization
- Hot reload capabilities
- Debugging tools

#### Staging Environment
- Production-like configuration
- Integration testing
- Performance testing
- User acceptance testing

#### Production Environment
- High availability setup
- Monitoring and alerting
- Backup and recovery
- Security hardening

### CI/CD Pipeline

#### Build Process
- Automated dependency installation
- Code compilation and testing
- Asset optimization
- Package creation

#### Deployment Process
- Environment-specific deployment
- Blue-green deployment
- Rollback procedures
- Health checks

#### Quality Gates
- Code quality checks
- Security scanning
- Performance testing
- Documentation validation

## 🔒 Security

### Authentication & Authorization
- JWT token-based authentication
- Role-based access control
- Multi-factor authentication
- Session management

### Data Protection
- Encryption at rest and in transit
- Secure credential management
- Data privacy compliance
- Audit logging

### Infrastructure Security
- Network security groups
- Web application firewall
- DDoS protection
- Security monitoring

## 📊 Monitoring & Observability

### Application Monitoring
- Performance metrics
- Error tracking
- User analytics
- Business metrics

### Infrastructure Monitoring
- Resource utilization
- Cost monitoring
- Availability metrics
- Capacity planning

### Logging & Tracing
- Centralized logging
- Distributed tracing
- Log analysis
- Alert management

## 🤝 Contributing

### Development Workflow
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests and documentation
5. Submit a pull request

### Code Review Process
- Automated checks (linting, testing)
- Peer code review
- Architecture review for major changes
- Security review for sensitive changes

### Release Process
- Semantic versioning
- Changelog maintenance
- Release notes
- Deployment coordination

## 📞 Support

### Getting Help
- **Documentation**: Comprehensive guides and references
- **Issues**: GitHub issue tracking
- **Discussions**: Community forums
- **Email**: Direct support contact

### Community
- **Contributors**: Active development community
- **Users**: Platform users and feedback
- **Partners**: Integration partners
- **Events**: Community events and meetups

## 📄 License

This project is licensed under the [MIT License](LICENSE). See the LICENSE file for details.

## 🙏 Acknowledgments

- **Contributors**: All contributors to the Flying Darts Turbo platform
- **Open Source**: Open source libraries and tools used
- **Community**: The darts gaming community
- **Users**: Platform users and their feedback

---

*This documentation is maintained by the Flying Darts Documentation Agent. For questions or contributions, please see the [Contributing Guide](CONTRIBUTING.md).*