# Workflow Validation System

This directory contains a comprehensive validation system for workflow YAML files. The system includes a JSON schema and validation tools to ensure your workflow definitions are correct and complete.

## 📁 Files

- `states.yml` - The workflow definition file (example)
- `workflow-schema.json` - JSON schema for validating workflow files
- `validate-workflow.py` - Python validation script
- `validate.sh` - Shell script wrapper for easy validation
- `requirements.txt` - Python dependencies
- `README.md` - This documentation

## 🚀 Quick Start

### Using the Shell Script (Recommended)

```bash
# Validate the default states.yml file
./validate.sh

# Validate a custom YAML file
./validate.sh my-workflow.yml

# Use a custom schema
./validate.sh -s custom-schema.json my-workflow.yml

# Show help
./validate.sh --help
```

### Using Python Directly

```bash
# Install dependencies
pip install -r requirements.txt

# Run validation
python3 validate-workflow.py states.yml
```

## 📋 Schema Validation

The JSON schema validates the following aspects of your workflow:

### Required Fields

- `workflow.name` - Must be a valid identifier
- `workflow.description` - Non-empty description
- `workflow.version` - Semantic version (e.g., "1.0.0")
- `workflow.events` - Array of event definitions
- `workflow.states` - Array of state definitions

### Event Structure

```yaml
events:
  - name: "EventName" # Must be valid identifier
    description: "Description" # Non-empty string
```

### State Structure

```yaml
states:
  - id: "state-id" # kebab-case identifier
    name: "StateName" # Valid identifier
    description: "Description" # Non-empty string
    isInitial: true # Optional boolean
    isFinal: true # Optional boolean
    actions: # Optional array
      - type: "log"
        level: "info|success|warning|error"
        message: "Log message"
      - type: "custom-action"
        question: "User prompt" # For prompt actions
    transitions: # Required for non-final states
      - event: "EventName"
        target: "target-state-id"
        condition: "optional-condition"
        description: "Optional description"
```

### Code Generation Configuration

```yaml
codeGeneration:
  language: "csharp|typescript|python|java"
  namespace: "Namespace.Name"
  baseClass: "BaseClassName"
  generateInterfaces: true
  generateTests: true
  customActions:
    - name: "action-name"
      description: "Action description"
      returnType: "string|int|bool|float|void"
      parameters:
        - name: "paramName"
          type: "string|int|bool|float"
```

## 🔍 Additional Validations

Beyond schema validation, the system performs these additional checks:

1. **Exactly one initial state** - Workflows must have exactly one state marked as `isInitial: true`
2. **At least one final state** - Workflows must have at least one state marked as `isFinal: true`
3. **Valid transition targets** - All transition targets must reference existing state IDs
4. **Valid event references** - All transition events must be defined in the events array
5. **Unique state IDs** - No duplicate state IDs are allowed
6. **Final state constraints** - Final states cannot have transitions

## 🛠️ Customization

### Extending the Schema

To add new validation rules, modify `workflow-schema.json`:

```json
{
  "properties": {
    "workflow": {
      "properties": {
        "yourNewField": {
          "type": "string",
          "description": "Your new field description"
        }
      }
    }
  }
}
```

### Adding Custom Validations

To add custom validation logic, modify `validate-workflow.py`:

```python
def perform_additional_validations(data: Dict[str, Any]) -> bool:
    # Your custom validation logic here
    workflow = data.get('workflow', {})

    # Example: Check for specific naming conventions
    for state in workflow.get('states', []):
        if not state['name'].endswith('State'):
            print(f"❌ State names should end with 'State': {state['name']}")
            return False

    return True
```

## 🐛 Troubleshooting

### Common Issues

1. **"Validation error: Expected exactly 1 initial state"**

   - Ensure exactly one state has `isInitial: true`

2. **"Validation error: No final states found"**

   - Add at least one state with `isFinal: true`

3. **"Validation error: Transition target 'xyz' does not exist"**

   - Check that all transition targets reference valid state IDs

4. **"Validation error: Event 'xyz' is not defined"**
   - Ensure all transition events are defined in the events array

### Debug Mode

For detailed validation output, you can modify the Python script to include debug logging:

```python
import logging
logging.basicConfig(level=logging.DEBUG)
```

## 📝 Best Practices

1. **Use descriptive names** - State and event names should clearly describe their purpose
2. **Follow naming conventions** - Use kebab-case for IDs, PascalCase for names
3. **Include descriptions** - Always provide meaningful descriptions for states and events
4. **Test transitions** - Ensure all possible paths through your workflow are valid
5. **Version your workflows** - Use semantic versioning for workflow changes

## 🔗 Integration

### CI/CD Integration

Add validation to your CI/CD pipeline:

```yaml
# GitHub Actions example
- name: Validate Workflow
  run: |
    cd scripts/beachball
    ./validate.sh
```

### Pre-commit Hook

Add to your pre-commit configuration:

```yaml
# .pre-commit-config.yaml
repos:
  - repo: local
    hooks:
      - id: validate-workflow
        name: Validate Workflow YAML
        entry: scripts/beachball/validate.sh
        language: script
        files: \.yml$
```

## 📄 License

This validation system is part of the FlyingDarts project and follows the same licensing terms.
