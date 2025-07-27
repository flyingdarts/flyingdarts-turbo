# Flying Darts Angular App (fd-app)

## Overview

The Flying Darts Angular App is a modern web-based darts application built with Angular 18. This application provides a comprehensive darts gaming experience with real-time multiplayer capabilities, user authentication, and social features.

This application is responsible for:
- Providing a web-based interface for darts games
- Managing user authentication and authorization
- Supporting real-time multiplayer gameplay
- Handling game state and scoring
- Integrating with backend services via WebSocket and REST APIs
- Providing social features like friends management
- Supporting voice and video communication during games

## Features

- **Real-time Multiplayer Gaming**: Live multiplayer darts games with WebSocket communication
- **User Authentication**: Secure authentication using Authress
- **Game Management**: Complete game lifecycle management (create, join, play, score)
- **Social Features**: Friends management and social interactions
- **Voice/Video Communication**: Integrated voice and video chat using Dyte
- **Responsive Design**: Modern, responsive UI that works on desktop and mobile
- **State Management**: NgRx-based state management for complex application state
- **Real-time Updates**: Live game updates and player status
- **Cross-platform**: Web-based application accessible from any modern browser
- **Progressive Web App**: PWA capabilities for enhanced user experience

## Prerequisites

- Node.js (v18 or higher)
- npm or yarn package manager
- Angular CLI (v18 or higher)
- Modern web browser with WebSocket support
- Authress account for authentication

## Installation

1. Clone the monorepo and navigate to the Angular app:
```bash
cd apps/frontend/angular/fd-app
```

2. Install dependencies:
```bash
npm install
```

3. Configure environment variables:
```bash
cp src/environments/environment.example.ts src/environments/environment.ts
```

4. Update the environment configuration with your settings.

## Usage

### Development Server

Start the development server:

```bash
npm start
```

The application will be available at `http://localhost:4200`.

### Building for Production

Build the application for production:

```bash
npm run build:prod
```

The built files will be in the `dist/` directory.

### Testing

Run the test suite:

```bash
npm test
```

Run tests in watch mode:

```bash
npm run test:watch
```

## Application Structure

### Core Architecture

The application follows Angular best practices with a modular architecture:

```
src/
├── app/
│   ├── core/                    # Core application services
│   ├── shared/                  # Shared components and utilities
│   ├── features/                # Feature modules
│   │   ├── auth/               # Authentication module
│   │   ├── game/               # Game management module
│   │   ├── friends/            # Friends management module
│   │   └── profile/            # User profile module
│   ├── store/                  # NgRx state management
│   └── app.component.*         # Root application component
├── assets/                     # Static assets
├── environments/               # Environment configurations
└── sdk/                       # API SDK and services
```

### Key Components

#### Core Services

##### `AuthService`

Handles user authentication and authorization.

**Methods:**
- `login()`: Authenticate user with Authress
- `logout()`: Logout current user
- `getCurrentUser()`: Get current authenticated user
- `isAuthenticated()`: Check if user is authenticated

##### `GameService`

Manages game state and interactions.

**Methods:**
- `createGame()`: Create a new darts game
- `joinGame()`: Join an existing game
- `updateScore()`: Update player score
- `getGameState()`: Get current game state

##### `WebSocketService`

Handles real-time communication with backend services.

**Methods:**
- `connect()`: Establish WebSocket connection
- `disconnect()`: Close WebSocket connection
- `sendMessage()`: Send message to server
- `subscribe()`: Subscribe to real-time updates

#### Feature Modules

##### Authentication Module (`auth/`)

- Login/Logout components
- User registration
- Password reset
- Profile management

##### Game Module (`game/`)

- Game creation and joining
- Score tracking
- Game state management
- Real-time game updates

##### Friends Module (`friends/`)

- Friends list management
- Friend requests
- Social interactions
- Online status

##### Profile Module (`profile/`)

- User profile management
- Game statistics
- Achievement tracking
- Settings management

### State Management

The application uses NgRx for state management:

#### Store Structure

```typescript
interface AppState {
  auth: AuthState;
  game: GameState;
  friends: FriendsState;
  profile: ProfileState;
}
```

#### Key Actions

- `AuthActions`: Authentication-related actions
- `GameActions`: Game state management actions
- `FriendsActions`: Friends management actions
- `ProfileActions`: Profile management actions

#### Effects

- `AuthEffects`: Handle authentication side effects
- `GameEffects`: Handle game-related side effects
- `WebSocketEffects`: Handle real-time communication

## Configuration

### Environment Configuration

The application uses environment-specific configuration:

#### Development Environment (`environment.ts`)

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:3000',
  wsUrl: 'ws://localhost:3000',
  authressUrl: 'https://your-domain.authress.io',
  dyteApiKey: 'your-dyte-api-key'
};
```

#### Production Environment (`environment.prod.ts`)

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://api.flyingdarts.com',
  wsUrl: 'wss://api.flyingdarts.com',
  authressUrl: 'https://your-domain.authress.io',
  dyteApiKey: 'your-dyte-api-key'
};
```

### Build Configuration

#### Angular Configuration (`angular.json`)

- Development and production build configurations
- Asset management
- Style and script optimization
- PWA configuration

#### TypeScript Configuration (`tsconfig.json`)

- TypeScript compiler options
- Path mapping
- Strict type checking

## Dependencies

### Core Dependencies

- **Angular 18**: Core framework
- **NgRx**: State management
- **RxJS**: Reactive programming
- **Bootstrap**: UI framework
- **Authress Angular**: Authentication
- **Dyte SDK**: Voice/video communication

### Development Dependencies

- **Angular CLI**: Development tools
- **Jest**: Testing framework
- **ESLint**: Code linting
- **TypeScript**: Type checking

## Development

### Project Structure

```
fd-app/
├── src/
│   ├── app/                     # Application source code
│   │   ├── core/               # Core services and guards
│   │   ├── shared/             # Shared components and pipes
│   │   ├── features/           # Feature modules
│   │   ├── store/              # NgRx state management
│   │   └── app.component.*     # Root component
│   ├── assets/                 # Static assets
│   ├── environments/           # Environment configurations
│   └── sdk/                   # API SDK
├── public/                     # Public assets
├── package.json                # Dependencies and scripts
├── angular.json               # Angular CLI configuration
├── tsconfig.json              # TypeScript configuration
└── jest.config.ts             # Jest testing configuration
```

### Code Organization

#### Feature-based Architecture

Each feature is organized as a separate module with its own:
- Components
- Services
- Guards
- Effects
- Reducers

#### Shared Resources

Common utilities and components are shared across features:
- Pipes
- Directives
- Guards
- Interceptors
- Models

### Testing Strategy

#### Unit Tests

- Component testing with Angular testing utilities
- Service testing with dependency injection
- Effect testing with NgRx testing utilities
- Reducer testing with pure functions

#### Integration Tests

- Module integration testing
- API integration testing
- WebSocket communication testing

#### E2E Tests

- User journey testing
- Cross-browser compatibility
- Performance testing

### Code Quality

The project uses:
- **ESLint**: Code linting and style enforcement
- **Prettier**: Code formatting
- **Husky**: Git hooks for quality checks
- **Jest**: Unit and integration testing

Run quality checks:

```bash
npm run lint
npm run test
npm run build
```

## User Interface

### Design System

The application uses Bootstrap 5 as the foundation with custom styling:

#### Components

- **Navigation**: Responsive navigation with user menu
- **Game Board**: Interactive darts board with scoring
- **Chat Interface**: Real-time messaging during games
- **Friends List**: Social features and friend management
- **Profile Dashboard**: User statistics and achievements

#### Responsive Design

- Mobile-first approach
- Tablet and desktop optimizations
- Touch-friendly interactions
- Adaptive layouts

### Accessibility

- WCAG 2.1 AA compliance
- Keyboard navigation support
- Screen reader compatibility
- High contrast mode support

## Integration

### Backend Services

#### REST API Integration

- Game management endpoints
- User profile endpoints
- Friends management endpoints
- Statistics and achievements

#### WebSocket Integration

- Real-time game updates
- Live chat functionality
- Player presence and status
- Game state synchronization

### Third-party Services

#### Authress

- User authentication
- Social login providers
- Role-based access control
- User management

#### Dyte

- Voice and video communication
- Screen sharing
- Recording capabilities
- Meeting management

## Performance Optimization

### Build Optimization

- Tree shaking for unused code
- Code splitting for lazy loading
- Asset optimization and compression
- Service worker for caching

### Runtime Optimization

- OnPush change detection strategy
- Lazy loading of modules
- Virtual scrolling for large lists
- Memory leak prevention

### Monitoring

- Performance metrics tracking
- Error monitoring and reporting
- User analytics
- Real user monitoring (RUM)

## Security

### Authentication

- JWT token management
- Secure token storage
- Automatic token refresh
- Session management

### Data Protection

- Input validation and sanitization
- XSS prevention
- CSRF protection
- Secure communication (HTTPS/WSS)

### Privacy

- GDPR compliance
- Data minimization
- User consent management
- Privacy policy integration

## Deployment

### Build Process

1. **Development Build**:
   ```bash
   npm run build
   ```

2. **Production Build**:
   ```bash
   npm run build:prod
   ```

3. **Testing**:
   ```bash
   npm test
   npm run test:ci
   ```

### Deployment Options

#### Static Hosting

- AWS S3 + CloudFront
- Netlify
- Vercel
- GitHub Pages

#### Container Deployment

- Docker containerization
- Kubernetes deployment
- CI/CD pipeline integration

### Environment Management

- Environment-specific configurations
- Feature flags
- A/B testing support
- Blue-green deployment

## Troubleshooting

### Common Issues

1. **Build Failures**: Check TypeScript errors and dependencies
2. **Runtime Errors**: Check browser console and network requests
3. **Authentication Issues**: Verify Authress configuration
4. **WebSocket Connection**: Check network connectivity and CORS

### Debugging

Enable debug logging:

```typescript
// In environment.ts
export const environment = {
  debug: true,
  // ... other config
};
```

### Performance Issues

- Monitor bundle size
- Check lazy loading implementation
- Analyze change detection cycles
- Review memory usage

## Related Projects

- **flyingdarts-backend**: Backend services and APIs
- **flyingdarts-mobile**: Flutter mobile application
- **flyingdarts-config**: Shared configuration package
- **flyingdarts-sdk**: API client SDK

## Contributing

1. Follow Angular style guide and best practices
2. Write comprehensive tests for new features
3. Update documentation for API changes
4. Ensure accessibility compliance
5. Follow the monorepo coding standards

## License

Part of the Flying Darts Turbo monorepo. See root LICENSE file for details.