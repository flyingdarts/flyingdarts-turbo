# API Reference - {{projectName}}

{{#if overview}}
## Overview

{{overview}}
{{/if}}

{{#if description}}
{{description}}
{{/if}}

## Table of Contents

{{#if namespaces}}
{{#each namespaces}}
- [{{name}}](#{{anchor}})
{{/each}}
{{/if}}

{{#if classes}}
{{#each classes}}
- [{{name}}](#{{anchor}})
{{/each}}
{{/if}}

{{#if interfaces}}
{{#each interfaces}}
- [{{name}}](#{{anchor}})
{{/each}}
{{/if}}

{{#if functions}}
{{#each functions}}
- [{{name}}](#{{anchor}})
{{/each}}
{{/if}}

{{#if types}}
{{#each types}}
- [{{name}}](#{{anchor}})
{{/each}}
{{/if}}

---

{{#if namespaces}}
## Namespaces

{{#each namespaces}}
### {{name}}

{{#if description}}{{description}}{{/if}}

{{#if classes}}
**Classes**:
{{#each classes}}
- [{{name}}]({{link}}) - {{description}}
{{/each}}
{{/if}}

{{#if interfaces}}
**Interfaces**:
{{#each interfaces}}
- [{{name}}]({{link}}) - {{description}}
{{/each}}
{{/if}}

{{#if functions}}
**Functions**:
{{#each functions}}
- [{{name}}]({{link}}) - {{description}}
{{/each}}
{{/if}}

---
{{/each}}
{{/if}}

{{#if classes}}
## Classes

{{#each classes}}
### {{name}}

{{#if description}}{{description}}{{/if}}

{{#if inheritance}}
**Inheritance**: {{inheritance}}
{{/if}}

{{#if summary}}
**Summary**: {{summary}}
{{/if}}

{{#if properties}}
**Properties**: {{properties}}
{{/if}}

{{#if methods}}
**Methods**: {{methods}}
{{/if}}

[View Full Documentation]({{link}})

---
{{/each}}
{{/if}}

{{#if interfaces}}
## Interfaces

{{#each interfaces}}
### {{name}}

{{#if description}}{{description}}{{/if}}

{{#if extends}}
**Extends**: {{extends}}
{{/if}}

{{#if properties}}
**Properties**: {{properties}}
{{/if}}

{{#if methods}}
**Methods**: {{methods}}
{{/if}}

[View Full Documentation]({{link}})

---
{{/each}}
{{/if}}

{{#if functions}}
## Functions

{{#each functions}}
### {{name}}({{#each parameters}}{{name}}{{#unless @last}}, {{/unless}}{{/each}}): {{returnType}}

{{#if description}}{{description}}{{/if}}

{{#if parameters}}
**Parameters**:
{{#each parameters}}
- `{{name}}` ({{type}}){{#if description}}: {{description}}{{/if}}
{{/each}}
{{/if}}

{{#if returns}}
**Returns**: {{returns}}
{{/if}}

{{#if example}}
**Example**:
```{{language}}
{{example}}
```
{{/if}}

---
{{/each}}
{{/if}}

{{#if types}}
## Types

{{#each types}}
### {{name}}

{{#if description}}{{description}}{{/if}}

{{#if definition}}
**Definition**:
```{{language}}
{{definition}}
```
{{/if}}

{{#if example}}
**Example**:
```{{language}}
{{example}}
```
{{/if}}

---
{{/each}}
{{/if}}

{{#if constants}}
## Constants

{{#each constants}}
### {{name}}: {{type}}

{{#if description}}{{description}}{{/if}}

**Value**: `{{value}}`

{{#if example}}
**Example**:
```{{language}}
{{example}}
```
{{/if}}

---
{{/each}}
{{/if}}

{{#if enums}}
## Enums

{{#each enums}}
### {{name}}

{{#if description}}{{description}}{{/if}}

**Values**:
{{#each values}}
- `{{name}}`{{#if description}} - {{description}}{{/if}}{{#if value}} = {{value}}{{/if}}
{{/each}}

{{#if example}}
**Example**:
```{{language}}
{{example}}
```
{{/if}}

---
{{/each}}
{{/if}}

{{#if dependencies}}
## Dependencies

{{#if internalDependencies}}
### Internal Dependencies
{{#each internalDependencies}}
- [{{name}}]({{link}}) - {{description}}
{{/each}}
{{/if}}

{{#if externalDependencies}}
### External Dependencies
{{#each externalDependencies}}
- **{{name}}** `{{version}}` - {{description}}
{{/each}}
{{/if}}
{{/if}}

{{#if examples}}
## Examples

{{#each examples}}
### {{title}}

{{description}}

```{{language}}
{{code}}
```
{{/each}}
{{/if}}

{{#if migration}}
## Migration Guide

{{#if breakingChanges}}
### Breaking Changes

{{#each breakingChanges}}
#### {{version}}

{{description}}

**Before**:
```{{language}}
{{before}}
```

**After**:
```{{language}}
{{after}}
```
{{/each}}
{{/if}}

{{#if deprecations}}
### Deprecations

{{#each deprecations}}
- `{{name}}` - {{description}} (Use `{{replacement}}` instead)
{{/each}}
{{/if}}
{{/if}}

---

*Generated on {{generatedDate}} by Flyingdarts Documentation Agent*

**Version**: {{version}}  
**Last Updated**: {{lastUpdated}} 
