# {{className}}

{{#if overview}}
## Overview

{{overview}}
{{/if}}

{{#if description}}
{{description}}
{{/if}}

{{#if inheritance}}
## Inheritance

{{#if extends}}
- **Extends**: [{{extends}}]({{extendsLink}})
{{/if}}
{{#if implements}}
- **Implements**: 
{{#each implements}}
  - [{{this}}]({{link}})
{{/each}}
{{/if}}
{{/if}}

{{#if properties}}
## Properties

{{#each properties}}
### {{name}}: {{type}}

{{#if access}}- **Access**: `{{access}}`{{/if}}
{{#if description}}- **Description**: {{description}}{{/if}}
{{#if defaultValue}}- **Default Value**: `{{defaultValue}}`{{/if}}
{{#if constraints}}- **Constraints**: {{constraints}}{{/if}}
{{#if readonly}}- **Read-only**: Yes{{/if}}
{{#if required}}- **Required**: Yes{{/if}}

{{#if example}}
**Example**:
```{{language}}
{{example}}
```
{{/if}}

---
{{/each}}
{{/if}}

{{#if methods}}
## Methods

{{#each methods}}
### {{name}}({{#each parameters}}{{name}}{{#unless @last}}, {{/unless}}{{/each}}): {{returnType}}

{{#if description}}{{description}}{{/if}}

{{#if parameters}}
**Parameters**:
{{#each parameters}}
- `{{name}}` ({{type}}){{#if description}}: {{description}}{{/if}}{{#if optional}} - Optional{{/if}}{{#if defaultValue}} - Default: `{{defaultValue}}`{{/if}}
{{/each}}
{{/if}}

{{#if returns}}
**Returns**: {{returns}}
{{/if}}

{{#if throws}}
**Throws**:
{{#each throws}}
- `{{type}}`: {{description}}
{{/each}}
{{/if}}

{{#if example}}
**Example**:
```{{language}}
{{example}}
```
{{/if}}

{{#if overloads}}
**Overloads**:
{{#each overloads}}
- `{{signature}}` - {{description}}
{{/each}}
{{/if}}

---
{{/each}}
{{/if}}

{{#if constructors}}
## Constructors

{{#each constructors}}
### {{name}}({{#each parameters}}{{name}}{{#unless @last}}, {{/unless}}{{/each}})

{{#if description}}{{description}}{{/if}}

{{#if parameters}}
**Parameters**:
{{#each parameters}}
- `{{name}}` ({{type}}){{#if description}}: {{description}}{{/if}}{{#if optional}} - Optional{{/if}}{{#if defaultValue}} - Default: `{{defaultValue}}`{{/if}}
{{/each}}
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

{{#if events}}
## Events

{{#each events}}
### {{name}}

{{#if description}}{{description}}{{/if}}

**Event Type**: `{{type}}`

{{#if parameters}}
**Event Parameters**:
{{#each parameters}}
- `{{name}}` ({{type}}){{#if description}}: {{description}}{{/if}}
{{/each}}
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

{{#if usage}}
## Usage Examples

{{#each usage}}
### {{title}}

{{description}}

```{{language}}
{{code}}
```
{{/each}}
{{/if}}

{{#if notes}}
## Notes

{{#each notes}}
- {{this}}
{{/each}}
{{/if}}

{{#if seeAlso}}
## See Also

{{#each seeAlso}}
- [{{name}}]({{link}}) - {{description}}
{{/each}}
{{/if}}

---

*Generated on {{generatedDate}} by Flying Darts Documentation Agent* 