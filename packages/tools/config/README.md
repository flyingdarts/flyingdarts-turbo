# @flyingdarts/config

This package provides standardized TypeScript configurations for the FlyingDarts monorepo.

## Available Configurations

### `tsconfig.base.json`
The main base configuration with strict type checking and modern ES2022 features. Suitable for most TypeScript packages.

**Key Features:**
- ES2022 target and module system
- Strict type checking enabled
- Source maps and declaration maps
- Modern module resolution
- Comprehensive type safety options

### `tsconfig.angular.json`
Specialized configuration for Angular applications.

**Extends:** `tsconfig.base.json`
**Additions:**
- DOM library support
- Angular-specific compiler options
- Strict template checking

### `tsconfig.node.json`
Configuration for Node.js applications and tools.

**Extends:** `tsconfig.base.json`
**Additions:**
- CommonJS module system
- Node.js-specific optimizations

### `tsconfig.test.json`
Configuration for test files.

**Extends:** `tsconfig.base.json`
**Additions:**
- Jest and Node.js types
- Test file patterns

## Usage

### For General TypeScript Packages
```json
{
  "extends": "../../packages/tools/config/tsconfig.base.json"
}
```

### For Angular Applications
```json
{
  "extends": "../../packages/tools/config/src/tsconfig.angular.json"
}
```

### For Node.js Applications
```json
{
  "extends": "../../packages/tools/config/tsconfig.node.json"
}
```

### For Test Files
```json
{
  "extends": "../../packages/tools/config/tsconfig.test.json"
}
```

## Package-Specific Overrides

You can add package-specific compiler options by extending the base configuration:

```json
{
  "extends": "../../packages/tools/config/tsconfig.base.json",
  "compilerOptions": {
    "outDir": "./custom-dist",
    "jsx": "preserve"
  }
}
```

## Key Settings Explained

### Strict Type Checking
- `strict: true` - Enables all strict type checking options
- `noImplicitOverride` - Requires explicit override keyword
- `noPropertyAccessFromIndexSignature` - Prevents unsafe property access
- `noImplicitReturns` - Ensures all code paths return a value
- `noFallthroughCasesInSwitch` - Prevents fallthrough in switch statements

### Module Resolution
- `moduleResolution: "node"` - Uses Node.js module resolution
- `esModuleInterop: true` - Enables ES module interop
- `allowSyntheticDefaultImports: true` - Allows default imports from modules without default exports
- `resolveJsonModule: true` - Allows importing JSON files

### Output Settings
- `declaration: true` - Generates .d.ts files
- `declarationMap: true` - Generates source maps for declarations
- `sourceMap: true` - Generates source maps for debugging

### Code Quality
- `forceConsistentCasingInFileNames: true` - Ensures consistent file naming
- `skipLibCheck: true` - Skips type checking of declaration files
- `isolatedModules: true` - Ensures each file can be safely transpiled independently 