# Flying Darts Documentation Agent

This directory contains the specification and implementation for a comprehensive background documentation agent that automatically generates and maintains documentation for the entire Flying Darts Turbo monorepo.

## Overview

The Documentation Agent is designed to:

- **Automatically discover** all projects in the monorepo
- **Generate comprehensive README.md files** for each project
- **Extract and document** all classes, properties, and methods
- **Maintain cross-references** between related projects
- **Ensure documentation quality** through validation and coverage metrics
- **Support multiple languages** including Rust, C#, Dart, and TypeScript

## Files

### Specification

- [`documentation-agent-specification.md`](documentation-agent-specification.md) - Complete specification for the documentation agent
- [`docs-agent.config.json`](docs-agent.config.json) - Sample configuration file
- [`docs-agent-example.js`](docs-agent-example.js) - Example implementation

### Templates

- [`templates/readme.md`](templates/readme.md) - Template for project README files
- [`templates/class.md`](templates/class.md) - Template for class documentation
- [`templates/api.md`](templates/api.md) - Template for API documentation
- [`templates/index.md`](templates/index.md) - Template for main documentation index

## Quick Start

### 1. Configuration

The agent uses a JSON configuration file (`docs-agent.config.json`) that defines:

- **Templates**: Paths to documentation templates
- **Rules**: Documentation generation rules and quality standards
- **Output**: Where generated documentation should be placed
- **Languages**: Supported programming languages and their parsers
- **Quality**: Coverage thresholds and validation rules

### 2. Project Discovery

The agent automatically discovers projects by scanning:

- `apps/` - Applications (backend, frontend, tools)
- `packages/` - Shared libraries and components
- `scripts/` - Development and build tools

### 3. Documentation Generation

For each discovered project, the agent generates:

- **README.md** - Project overview, installation, usage, and API reference
- **docs/api/** - Detailed API documentation
- **docs/api/classes/** - Individual class documentation
- **docs/examples/** - Code examples and usage patterns

### 4. Quality Assurance

The agent validates generated documentation for:

- **Completeness** - All public APIs documented
- **Accuracy** - Cross-references are valid
- **Consistency** - Consistent formatting and style
- **Coverage** - Minimum documentation coverage thresholds

## Supported Languages

### Rust
- Parses `Cargo.toml` for dependencies and metadata
- Extracts structs, enums, traits, and implementations
- Processes documentation comments (`///` and `//!`)
- Handles module organization

### C#/.NET
- Parses `.csproj` files for project information
- Extracts classes, interfaces, properties, and methods
- Processes XML documentation comments
- Handles inheritance and interface implementations

### Dart/Flutter
- Parses `pubspec.yaml` for dependencies
- Extracts classes, mixins, and extensions
- Processes documentation comments (`///`)
- Handles constructor parameters

### TypeScript/JavaScript
- Parses `package.json` for project metadata
- Extracts classes, interfaces, and functions
- Processes JSDoc comments
- Handles decorators and type definitions

## Template System

The agent uses Handlebars-style templates for documentation generation:

### README Template
```markdown
# {{projectName}}

## Overview
{{overview}}

## Features
{{#each features}}
- {{this}}
{{/each}}

## Installation
{{installationCommand}}

## API Reference
For detailed API documentation, see [API Reference]({{apiReference}})
```

### Class Template
```markdown
# {{className}}

## Overview
{{overview}}

## Properties
{{#each properties}}
### {{name}}: {{type}}
- **Access**: `{{access}}`
- **Description**: {{description}}
{{/each}}

## Methods
{{#each methods}}
### {{name}}({{#each parameters}}{{name}}{{#unless @last}}, {{/unless}}{{/each}}): {{returnType}}
{{description}}
{{/each}}
```

## Example Usage

```javascript
const DocumentationAgent = require('./docs-agent-example.js');

const agent = new DocumentationAgent('docs/docs-agent.config.json');
agent.generateDocumentation()
    .then(() => console.log('Documentation generated successfully!'))
    .catch(console.error);
```

## Integration

### CI/CD Integration
The agent can be integrated into CI/CD pipelines to:

- Generate documentation on every commit
- Validate documentation quality
- Update documentation websites
- Track documentation coverage metrics

### IDE Integration
- Documentation preview in VS Code
- Quick navigation between code and docs
- Real-time documentation validation
- Auto-completion for documentation

### Git Integration
- Automatic documentation commits
- Documentation branching strategies
- Documentation review workflows
- Version-controlled documentation

## Quality Metrics

The agent tracks several quality metrics:

- **Coverage**: Percentage of public APIs documented
- **Completeness**: All required sections present
- **Accuracy**: Valid cross-references and links
- **Consistency**: Uniform formatting and style
- **Freshness**: Documentation matches current code

## Customization

### Custom Templates
Create custom templates by:

1. Adding new template files to `templates/`
2. Updating the configuration file
3. Implementing custom template logic

### Custom Parsers
Add support for new languages by:

1. Implementing language-specific parsers
2. Adding language configuration
3. Creating language-specific templates

### Custom Validation
Implement custom validation rules by:

1. Adding validation functions
2. Configuring validation rules
3. Setting quality thresholds

## Best Practices

### Documentation Standards
- Use clear, concise descriptions
- Include code examples where helpful
- Maintain consistent formatting
- Keep documentation up-to-date

### Template Design
- Make templates reusable across projects
- Use conditional blocks for optional content
- Include proper error handling
- Support multiple output formats

### Performance
- Use incremental updates when possible
- Cache parsed results
- Process files in parallel
- Optimize for large codebases

## Troubleshooting

### Common Issues

1. **Missing Configuration**: Ensure `docs-agent.config.json` exists and is valid
2. **Template Errors**: Check template syntax and variable names
3. **Parser Failures**: Verify source code syntax and structure
4. **Permission Errors**: Ensure write access to output directories

### Debug Mode
Enable debug logging by setting the log level in configuration:

```json
{
  "logging": {
    "level": "debug"
  }
}
```

## Contributing

To contribute to the documentation agent:

1. Follow the existing code style and patterns
2. Add tests for new functionality
3. Update documentation for new features
4. Ensure backward compatibility

## License

This documentation agent is part of the Flying Darts Turbo monorepo and follows the same licensing terms.

---

*For more information, see the [complete specification](documentation-agent-specification.md).* 