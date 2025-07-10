#!/usr/bin/env python3
"""
Workflow YAML Schema Validator

This script validates workflow YAML files against the JSON schema.
Usage: python validate-workflow.py [yaml_file]
"""

import sys
import yaml
import json
import jsonschema
from pathlib import Path
from typing import Dict, Any


def load_yaml_file(file_path: str) -> Dict[str, Any]:
    """Load and parse a YAML file."""
    try:
        with open(file_path, 'r', encoding='utf-8') as file:
            return yaml.safe_load(file)
    except yaml.YAMLError as e:
        print(f"❌ Error parsing YAML file: {e}")
        sys.exit(1)
    except FileNotFoundError:
        print(f"❌ File not found: {file_path}")
        sys.exit(1)


def load_schema_file(schema_path: str) -> Dict[str, Any]:
    """Load and parse the JSON schema file."""
    try:
        with open(schema_path, 'r', encoding='utf-8') as file:
            return json.load(file)
    except json.JSONDecodeError as e:
        print(f"❌ Error parsing JSON schema: {e}")
        sys.exit(1)
    except FileNotFoundError:
        print(f"❌ Schema file not found: {schema_path}")
        sys.exit(1)


def validate_workflow(data: Dict[str, Any], schema: Dict[str, Any]) -> bool:
    """Validate the workflow data against the schema."""
    try:
        jsonschema.validate(instance=data, schema=schema)
        return True
    except jsonschema.ValidationError as e:
        print(f"❌ Validation error: {e.message}")
        print(f"   Path: {' -> '.join(str(p) for p in e.path)}")
        return False
    except jsonschema.SchemaError as e:
        print(f"❌ Schema error: {e}")
        return False


def perform_additional_validations(data: Dict[str, Any]) -> bool:
    """Perform additional custom validations beyond schema validation."""
    workflow = data.get('workflow', {})
    states = workflow.get('states', [])
    events = workflow.get('events', [])
    
    # Check for exactly one initial state
    initial_states = [s for s in states if s.get('isInitial', False)]
    if len(initial_states) != 1:
        print(f"❌ Validation error: Expected exactly 1 initial state, found {len(initial_states)}")
        return False
    
    # Check for at least one final state
    final_states = [s for s in states if s.get('isFinal', False)]
    if len(final_states) == 0:
        print("❌ Validation error: No final states found")
        return False
    
    # Check that all transition targets exist
    state_ids = {s['id'] for s in states}
    event_names = {e['name'] for e in events}
    
    for state in states:
        if not state.get('isFinal', False):  # Skip final states
            transitions = state.get('transitions', [])
            for transition in transitions:
                target = transition.get('target')
                event = transition.get('event')
                
                if target and target not in state_ids:
                    print(f"❌ Validation error: Transition target '{target}' in state '{state['id']}' does not exist")
                    return False
                
                if event and event not in event_names:
                    print(f"❌ Validation error: Event '{event}' in state '{state['id']}' is not defined")
                    return False
    
    # Check for unique state IDs
    state_id_counts = {}
    for state in states:
        state_id = state['id']
        state_id_counts[state_id] = state_id_counts.get(state_id, 0) + 1
        if state_id_counts[state_id] > 1:
            print(f"❌ Validation error: Duplicate state ID '{state_id}' found")
            return False
    
    return True


def main():
    """Main validation function."""
    # Determine file paths
    script_dir = Path(__file__).parent
    yaml_file = sys.argv[1] if len(sys.argv) > 1 else script_dir / "states.yml"
    schema_file = script_dir / "workflow-schema.json"
    
    print(f"🔍 Validating workflow file: {yaml_file}")
    print(f"📋 Using schema: {schema_file}")
    print()
    
    # Load files
    data = load_yaml_file(str(yaml_file))
    schema = load_schema_file(str(schema_file))
    
    # Validate against schema
    print("📋 Validating against JSON schema...")
    if not validate_workflow(data, schema):
        print("❌ Schema validation failed!")
        sys.exit(1)
    print("✅ Schema validation passed!")
    
    # Perform additional validations
    print("🔍 Performing additional validations...")
    if not perform_additional_validations(data):
        print("❌ Additional validations failed!")
        sys.exit(1)
    print("✅ Additional validations passed!")
    
    print()
    print("🎉 All validations passed! The workflow file is valid.")


if __name__ == "__main__":
    main() 