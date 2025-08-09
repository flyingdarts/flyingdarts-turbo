# Flyingdarts Angular Web App

## Overview

The Flyingdarts Angular Web App is a modern web application built with Angular 18 that provides a comprehensive darts gaming platform accessible through web browsers. The application offers real-time multiplayer darts games, user management, and social features with a responsive design optimized for desktop and mobile devices.

This application is responsible for:
- Providing a web-based interface for darts gameplay
- Managing user authentication through Authress integration
- Enabling real-time multiplayer games via WebSocket connections
- Supporting multiple game modes including X01 variants
- Offering social features like friends management and friend requests
- Providing responsive design for various screen sizes
- Managing application state with NgRx store management

## Features

- **Modern Web Interface**: Built with Angular 18 and Material Design principles
- **Real-time Multiplayer**: WebSocket-based live gaming with other players
- **Authentication**: Secure login via Authress enterprise authentication
- **State Management**: Comprehensive state management using NgRx
- **Social Features**: Friends management, friend requests, and social interactions
- **Responsive Design**: Adaptive UI for desktop, tablet, and mobile devices
- **Video Conferencing**: Integrated Dyte video conferencing for live gameplay
- **Animations**: Lottie animations for enhanced user experience
- **Theme Support**: Light and dark theme modes
- **Progressive Web App**: PWA capabilities for enhanced mobile experience

## Prerequisites

- Node.js 18+ and npm
- Angular CLI 18+
- Modern web browser with ES2020 support
- Git for version control

## Installation

1. Clone the monorepo and navigate to the Angular app:
```bash
cd apps/frontend/angular/fd-app
```

2. Install dependencies:
```bash
npm install
```

3. Start the development server:
```bash
npm start
```

## Usage

### Development

Start the development server:

```bash
npm start
```

The application will be available at `http://localhost:4200/`.

### Production Build

Build the application for production:

```bash
npm run build:prod
```

### Testing

Run unit tests:

```bash
npm test
```

Run tests in watch mode:

```bash
npm test -- --watch
```

## Architecture

### Project Structure

```
src/
├── app/
│   ├── components/           # Reusable UI components
│   ├── features/            # Feature modules
│   ├── guards/              # Route guards
│   ├── interceptors/        # HTTP interceptors
│   ├── repositories/        # Data access layer
│   ├── requests/            # API request models
│   ├── resolvers/           # Route resolvers
│   ├── services/            # Business logic services
│   ├── state/               # NgRx state management
│   ├── dtos/                # Data transfer objects
│   ├── app.component.ts     # Root component
│   ├── app.component.html   # Root template
│   ├── app.module.ts        # Root module
│   └── app-routing.module.ts # Main routing
├── assets/                  # Static assets
├── environments/            # Environment configurations
├── sdk/                     # SDK configurations
├── main.ts                  # Application entry point
├── styles.scss              # Global styles
└── index.html               # HTML template
```

### Key Components

#### `AppComponent`

The root component that initializes the application and manages global state.

**Properties:**
- `title` (string): Application title
- `appStateLoading$` (Observable<boolean>): Loading state observable
- `themeMode$` (Observable<string>): Theme mode observable
- `lottieOptions` (AnimationOptions): Lottie animation configuration

**Methods:**

##### `constructor(store: Store)`

Initializes the component with NgRx store and dispatches initial actions.

**Parameters:**
- `store` (Store): NgRx store instance

#### `AppModule`

The root Angular module that configures the application.

**Imports:**
- `BrowserModule`: Browser-specific functionality
- `CommonModule`: Common Angular directives and pipes
- `AppRoutingModule`: Application routing
- `StoreModule`: NgRx store configuration
- `EffectsModule`: NgRx effects configuration
- `DyteComponentsModule`: Video conferencing components
- `AuthressModule`: Authentication module

**Providers:**
- HTTP client with interceptors
- Lottie animation options
- NgRx store devtools

### State Management

The application uses NgRx for comprehensive state management:

#### Store Structure

- **App State**: Global application state (loading, theme, etc.)
- **Game State**: Game-related state management
- **Friends State**: Friends and social features state

#### Key Actions

- `AppStateActions`: Global application actions
- `GameStateActions`: Game-related actions
- `FriendsStateActions`: Friends management actions

#### Effects

- `AppEffects`: Global application side effects
- `FriendsEffects`: Friends-related side effects

### Routing

The application uses Angular Router with feature-based routing:

- **Lazy Loading**: Feature modules are loaded on demand
- **Route Guards**: Authentication and authorization guards
- **Route Resolvers**: Data pre-loading for routes

## Dependencies

### Core Dependencies

- **@angular/core**: Angular core framework
- **@angular/common**: Common Angular utilities
- **@angular/router**: Angular routing
- **@angular/forms**: Form handling
- **@angular/animations**: Animation support

### State Management

- **@ngrx/store**: State management library
- **@ngrx/effects**: Side effect management
- **@ngrx/store-devtools**: Development tools

### UI and Styling

- **bootstrap**: CSS framework
- **bootstrap-icons**: Icon library
- **ngx-lottie**: Lottie animation integration

### Authentication

- **@mikepattyn/authress-angular**: Authress authentication integration

### Video Conferencing

- **@dytesdk/angular-ui-kit**: Dyte video conferencing components
- **@dytesdk/web-core**: Dyte core functionality

### Development Dependencies

- **@angular/cli**: Angular CLI tools
- **@angular-devkit/build-angular**: Build tools
- **jest**: Testing framework
- **eslint**: Code linting
- **typescript**: TypeScript compiler

## Configuration

### Environment Configuration

The application supports multiple environments:

- **development**: Development environment
- **production**: Production environment

Environment files are located in `src/environments/`.

### Build Configuration

Build configurations are defined in `angular.json`:

- **development**: Development build with source maps
- **production**: Production build with optimizations

### Authentication Configuration

```typescript
AuthressModule.forRoot({
  authressApiUrl: environment.authressLoginUrl,
  applicationId: environment.authressAppId,
})
```

## Development

### Code Generation

Generate new components, services, and modules:

```bash
ng generate component my-component
ng generate service my-service
ng generate module my-module
```

### Code Quality

The project uses:
- **ESLint**: Code linting and formatting
- **TypeScript**: Type safety
- **Jest**: Unit testing framework

Run linting:

```bash
npm run lint
```

### Testing

Run unit tests:

```bash
npm test
```

Run tests with coverage:

```bash
npm test -- --coverage
```

### Building

Build for development:

```bash
npm run build
```

Build for production:

```bash
npm run build:prod
```

## Deployment

### Static Hosting

1. Build the application:
```bash
npm run build:prod
```

2. Deploy the `dist/` directory to your web server.

### Docker Deployment

1. Build the Docker image:
```bash
docker build -t flyingdarts-web .
```

2. Run the container:
```bash
docker run -p 80:80 flyingdarts-web
```

### Cloud Deployment

The application can be deployed to various cloud platforms:

- **AWS S3 + CloudFront**: Static hosting with CDN
- **Azure Static Web Apps**: Azure hosting solution
- **Google Cloud Storage**: Google Cloud hosting
- **Vercel**: Serverless deployment platform
- **Netlify**: Static site hosting

## Related Projects

- **flyingdarts-mobile**: Flutter mobile application
- **flyingdarts-api**: Backend API services
- **flyingdarts-auth**: Authentication services
- **flyingdarts-websocket**: WebSocket communication

## Troubleshooting

### Common Issues

1. **Dependency Resolution**: Run `npm install` after dependency changes
2. **Build Errors**: Check TypeScript compilation errors
3. **Routing Issues**: Verify route configurations
4. **State Management**: Check NgRx store configuration
5. **Authentication**: Verify Authress configuration

### Debugging

Enable debug logging:

```typescript
// In your component
console.log('Debug message');
```

Use Angular DevTools for debugging:
- Install Angular DevTools browser extension
- Use NgRx DevTools for state debugging

### Performance

- **Bundle Size**: Monitor bundle size with `npm run build:prod`
- **Lazy Loading**: Ensure feature modules are lazy loaded
- **Change Detection**: Optimize change detection strategies
- **Memory Management**: Monitor memory usage in browser dev tools

## Security Considerations

- **Authentication**: All API calls require valid authentication tokens
- **HTTPS**: Use HTTPS in production environments
- **Input Validation**: Validate all user inputs
- **XSS Prevention**: Use Angular's built-in XSS protection
- **CSRF Protection**: Implement CSRF protection for API calls

## Contributing

1. Follow the monorepo coding standards
2. Add tests for new functionality
3. Update documentation for UI changes
4. Ensure all tests pass before submitting PR
5. Follow Angular best practices and conventions

## License

Part of the Flyingdarts Turbo monorepo. See root LICENSE file for details.
