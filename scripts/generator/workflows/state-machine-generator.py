#!/usr/bin/env python3
"""
State Machine Code Generator
Generates C# code and shell scripts from YAML state machine definitions following DDD and OOP best practices.

Usage:
    python state-machine-generator.py <yaml-file> [output-directory] [--shell]
"""

import yaml
import sys
import os
import re
from pathlib import Path
from typing import Dict, List, Any, Optional
from dataclasses import dataclass
from datetime import datetime
import argparse


@dataclass
class StateMachineConfig:
    """Configuration for state machine generation"""
    name: str
    description: str
    version: str
    namespace: str
    base_class: str
    generate_interfaces: bool
    generate_tests: bool
    language: str


@dataclass
class State:
    """Represents a state in the state machine"""
    id: str
    name: str
    description: str
    is_initial: bool = False
    is_final: bool = False
    actions: Optional[List[Dict[str, Any]]] = None
    transitions: Optional[List[Dict[str, Any]]] = None

    def __post_init__(self):
        if self.actions is None:
            self.actions = []
        if self.transitions is None:
            self.transitions = []


@dataclass
class Event:
    """Represents a domain event"""
    name: str
    description: str


@dataclass
class CustomAction:
    """Represents a custom action that needs implementation"""
    name: str
    description: str
    return_type: str
    parameters: Optional[List[Dict[str, str]]] = None

    def __post_init__(self):
        if self.parameters is None:
            self.parameters = []


class StateMachineGenerator:
    """Generates C# code and shell scripts from YAML state machine definitions"""
    
    def __init__(self, yaml_file: str, output_dir: str = "generated", generate_shell: bool = False):
        self.yaml_file = yaml_file
        self.output_dir = Path(output_dir)
        self.generate_shell = generate_shell
        self.config = None
        self.states = []
        self.events = []
        self.custom_actions = []
        
    def load_yaml(self) -> None:
        """Load and parse the YAML file"""
        try:
            with open(self.yaml_file, 'r', encoding='utf-8') as file:
                data = yaml.safe_load(file)
                
            workflow = data.get('workflow', {})
            
            # Load configuration
            code_gen = workflow.get('codeGeneration', {})
            self.config = StateMachineConfig(
                name=workflow.get('name', 'UnknownWorkflow'),
                description=workflow.get('description', ''),
                version=workflow.get('version', '1.0.0'),
                namespace=code_gen.get('namespace', 'Generated.Workflows'),
                base_class=code_gen.get('baseClass', 'StateMachineBase'),
                generate_interfaces=code_gen.get('generateInterfaces', True),
                generate_tests=code_gen.get('generateTests', True),
                language=code_gen.get('language', 'csharp')
            )
            
            # Load states
            for state_data in workflow.get('states', []):
                state = State(
                    id=state_data['id'],
                    name=state_data['name'],
                    description=state_data['description'],
                    is_initial=state_data.get('isInitial', False),
                    is_final=state_data.get('isFinal', False),
                    actions=state_data.get('actions', []),
                    transitions=state_data.get('transitions', [])
                )
                self.states.append(state)
            
            # Load events
            for event_data in workflow.get('events', []):
                event = Event(
                    name=event_data['name'],
                    description=event_data['description']
                )
                self.events.append(event)
            
            # Load custom actions
            for action_data in code_gen.get('customActions', []):
                action = CustomAction(
                    name=action_data['name'],
                    description=action_data['description'],
                    return_type=action_data['returnType'],
                    parameters=action_data.get('parameters', [])
                )
                self.custom_actions.append(action)
                
        except Exception as e:
            print(f"❌ Error loading YAML file: {e}")
            sys.exit(1)
    
    def to_pascal_case(self, text: str) -> str:
        """Convert text to PascalCase"""
        return ''.join(word.capitalize() for word in re.split(r'[-_\s]+', text))
    
    def to_camel_case(self, text: str) -> str:
        """Convert text to camelCase"""
        pascal = self.to_pascal_case(text)
        return pascal[0].lower() + pascal[1:]
    
    def to_snake_case(self, text: str) -> str:
        """Convert text to snake_case"""
        return re.sub(r'[-_\s]+', '_', text.lower())
    
    def to_upper_snake_case(self, text: str) -> str:
        """Convert text to UPPER_SNAKE_CASE"""
        return self.to_snake_case(text).upper()
    
    def generate_enum(self, name: str, values: List[str]) -> str:
        """Generate a C# enum"""
        enum_values = [f"    {self.to_pascal_case(value)}," for value in values]
        return f"""public enum {name}
{{
{chr(10).join(enum_values)}
}}"""
    
    def generate_workflow_result(self) -> str:
        """Generate WorkflowResult class"""
        return """public class WorkflowResult
{
    public string CurrentState { get; set; }
    public bool IsCompleted { get; set; }
    public bool IsError { get; set; }
    public string ErrorMessage { get; set; }
    public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();
    
    public static WorkflowResult Success(string state, Dictionary<string, object> data = null)
    {
        return new WorkflowResult
        {
            CurrentState = state,
            IsCompleted = false,
            IsError = false,
            Data = data ?? new Dictionary<string, object>()
        };
    }
    
    public static WorkflowResult Completed(string state, Dictionary<string, object> data = null)
    {
        return new WorkflowResult
        {
            CurrentState = state,
            IsCompleted = true,
            IsError = false,
            Data = data ?? new Dictionary<string, object>()
        };
    }
    
    public static WorkflowResult Error(string state, string message)
    {
        return new WorkflowResult
        {
            CurrentState = state,
            IsCompleted = true,
            IsError = true,
            ErrorMessage = message
        };
    }
}"""
    
    def generate_interfaces(self) -> str:
        """Generate interfaces for the state machine"""
        if not self.config:
            raise ValueError("Configuration not loaded")
            
        interface_name = f"I{self.config.name}"
        
        # Generate custom action interfaces
        action_interfaces = []
        for action in self.custom_actions:
            params = []
            for param in action.parameters:
                params.append(f"{param['type']} {self.to_camel_case(param['name'])}")
            
            param_str = ", ".join(params) if params else ""
            action_interfaces.append(f"""    Task<{action.return_type}> {self.to_pascal_case(action.name)}Async({param_str});""")
        
        return f"""public interface {interface_name}
{{
    Task<WorkflowResult> StartAsync();
    Task<WorkflowResult> HandleEventAsync(string eventName, object data = null);
    string CurrentState {{ get; }}
    bool IsCompleted {{ get; }}
}}

public interface I{self.config.name}Actions
{{
{chr(10).join(action_interfaces)}
}}"""
    
    def generate_state_machine_class(self) -> str:
        """Generate the main state machine class"""
        if not self.config:
            raise ValueError("Configuration not loaded")
            
        class_name = self.config.name
        
        # Generate state constants
        state_constants = []
        for state in self.states:
            state_constants.append(f'    public const string {self.to_pascal_case(state.id)} = "{state.id}";')
        
        # Generate state execution methods
        state_methods = []
        for state in self.states:
            method_name = f"Execute{self.to_pascal_case(state.id)}StateAsync"
            actions_code = self.generate_state_actions(state)
            transitions_code = self.generate_state_transitions(state)
            
            state_methods.append(f"""    private async Task<WorkflowResult> {method_name}Async()
    {{
        _logger.LogInformation("Entering state: {state.name}");
        
{actions_code}
        
{transitions_code}
    }}""")
        
        # Generate constructor and dependencies
        dependencies = []
        if any(action['type'] == 'log' for state in self.states for action in state.actions):
            dependencies.append("ILogger<{class_name}>")
        if self.custom_actions:
            dependencies.append(f"I{self.config.name}Actions")
        
        constructor_params = ", ".join(dependencies)
        constructor_body = "\n".join([f"        _{dep.split('<')[0].lower()} = {dep.split('<')[0].lower()};" for dep in dependencies])
        
        return f"""public class {class_name} : {self.config.base_class}, I{class_name}
{{
    // State constants
{chr(10).join(state_constants)}
    
    // Dependencies
{chr(10).join([f"    private readonly {dep} _{dep.split('<')[0].lower()};" for dep in dependencies])}
    
    public {class_name}({constructor_params})
    {{
{constructor_body}
    }}
    
    public async Task<WorkflowResult> StartAsync()
    {{
        _logger.LogInformation("🚀 Starting {self.config.name}");
        return await TransitionToAsync({self.to_pascal_case(self.get_initial_state().id)});
    }}
    
    public async Task<WorkflowResult> HandleEventAsync(string eventName, object data = null)
    {{
        _logger.LogInformation($"📨 Handling event: {{eventName}}");
        
        switch (CurrentState)
        {{
{self.generate_event_handlers()}
            default:
                throw new InvalidOperationException($"Unknown state: {{CurrentState}}");
        }}
    }}
    
    public string CurrentState => _currentState;
    public bool IsCompleted => _isCompleted;
    
{chr(10).join(state_methods)}
}}"""
    
    def generate_state_actions(self, state: State) -> str:
        """Generate code for state actions"""
        actions_code = []
        
        if not state.actions:
            return "        // No actions to execute"
        
        for action in state.actions:
            if action['type'] == 'log':
                level = action['level']
                message = action['message']
                actions_code.append(f'        _logger.Log{level.capitalize()}("{message}");')
            elif action['type'] == 'prompt-user':
                question = action.get('question', 'user_input')
                actions_code.append(f'        var userResponse = await _actions.{self.to_pascal_case(action["type"])}Async("{question}");')
            else:
                # Custom action
                actions_code.append(f'        var result = await _actions.{self.to_pascal_case(action["type"])}Async();')
        
        return "\n".join(actions_code) if actions_code else "        // No actions to execute"
    
    def generate_state_transitions(self, state: State) -> str:
        """Generate transition logic for a state"""
        if not state.transitions:
            return "        return WorkflowResult.Completed(CurrentState);"
        
        transition_code = []
        for transition in state.transitions:
            event_name = transition['event']
            target = transition['target']
            condition = transition.get('condition')
            
            if condition:
                transition_code.append(f"""        if ({condition})
        {{
            return await TransitionToAsync({self.to_pascal_case(target)});
        }}""")
            else:
                transition_code.append(f"""        return await TransitionToAsync({self.to_pascal_case(target)});""")
        
        return "\n".join(transition_code)
    
    def generate_event_handlers(self) -> str:
        """Generate event handler switch cases"""
        handlers = []
        for state in self.states:
            if state.transitions:
                case_body = []
                for transition in state.transitions:
                    event_name = transition['event']
                    target = transition['target']
                    case_body.append(f"                case \"{event_name}\":")
                    case_body.append(f"                    return await TransitionToAsync({self.to_pascal_case(target)});")
                
                if case_body:
                    handlers.append(f"""            case {self.to_pascal_case(state.id)}:
{chr(10).join(case_body)}
                break;""")
        
        return "\n".join(handlers)
    
    def get_initial_state(self) -> State:
        """Get the initial state"""
        for state in self.states:
            if state.is_initial:
                return state
        raise ValueError("No initial state found")
    
    def generate_base_class(self) -> str:
        """Generate the base state machine class"""
        if not self.config:
            raise ValueError("Configuration not loaded")
            
        return f"""public abstract class {self.config.base_class}
{{
    protected string _currentState;
    protected bool _isCompleted;
    protected readonly Dictionary<string, object> _stateData = new Dictionary<string, object>();
    
    protected async Task<WorkflowResult> TransitionToAsync(string newState)
    {{
        _currentState = newState;
        _isCompleted = false;
        
        // Execute the state
        var methodName = $"Execute{{newState}}StateAsync";
        var method = GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (method != null)
        {{
            return await (Task<WorkflowResult>)method.Invoke(this, null);
        }}
        
        return WorkflowResult.Success(newState);
    }}
    
    protected void SetStateData(string key, object value)
    {{
        _stateData[key] = value;
    }}
    
    protected T GetStateData<T>(string key, T defaultValue = default(T))
    {{
        return _stateData.TryGetValue(key, out var value) ? (T)value : defaultValue;
    }}
}}"""
    
    def generate_tests(self) -> str:
        """Generate unit tests for the state machine"""
        if not self.config:
            raise ValueError("Configuration not loaded")
            
        test_class_name = f"{self.config.name}Tests"
        
        test_methods = []
        for state in self.states:
            if not state.is_final:
                test_methods.append(f"""    [Fact]
    public async Task {self.to_pascal_case(state.id)}State_ShouldExecuteCorrectly()
    {{
        // Arrange
        var mockLogger = new Mock<ILogger<{self.config.name}>>();
        var mockActions = new Mock<I{self.config.name}Actions>();
        var workflow = new {self.config.name}(mockLogger.Object, mockActions.Object);
        
        // Act
        var result = await workflow.StartAsync();
        
        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsError);
    }}""")
        
        return f"""using Xunit;
using Moq;
using Microsoft.Extensions.Logging;

namespace {self.config.namespace}.Tests
{{
    public class {test_class_name}
    {{
{chr(10).join(test_methods)}
    }}
}}"""
    
    def generate_shell_script(self) -> str:
        """Generate a shell script implementation of the state machine"""
        if not self.config:
            raise ValueError("Configuration not loaded")
        
        script_name = self.to_snake_case(self.config.name)
        
        # Generate state constants
        state_constants = []
        for state in self.states:
            state_constants.append(f'{self.to_upper_snake_case(state.id)}="{state.id}"')
        
        # Generate state functions
        state_functions = []
        for state in self.states:
            function_name = f"execute_{self.to_snake_case(state.id)}_state"
            actions_code = self.generate_shell_state_actions(state)
            transitions_code = self.generate_shell_state_transitions(state)
            
            state_functions.append(f"""# Execute {state.name} state
{function_name}() {{
    echo "🔄 Entering state: {state.name}"
    
{actions_code}
    
{transitions_code}
}}""")
        
        # Generate main function
        main_function = self.generate_shell_main_function()
        
        # Generate helper functions
        helper_functions = self.generate_shell_helper_functions()
        
        return f"""#!/bin/bash
# {self.config.name} - {self.config.description}
# Generated State Machine Shell Script
# Version: {self.config.version}
# Generated on: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}

set -euo pipefail

# Colors for output
RED='\\033[0;31m'
GREEN='\\033[0;32m'
YELLOW='\\033[1;33m'
BLUE='\\033[0;34m'
NC='\\033[0m' # No Color

# State constants
{chr(10).join(state_constants)}

# Global variables
CURRENT_STATE=""
IS_COMPLETED=false
ERROR_MESSAGE=""
STATE_DATA=()

# Helper functions
{helper_functions}

# State execution functions
{chr(10).join(state_functions)}

# Main function
{main_function}

# Script entry point
if [[ "${{BASH_SOURCE[0]}}" == "$0" ]]; then
    main "$@"
fi"""
    
    def generate_shell_state_actions(self, state: State) -> str:
        """Generate shell script actions for a state"""
        actions_code = []
        
        if not state.actions:
            return "    # No actions to execute"
        
        for action in state.actions:
            if action['type'] == 'log':
                level = action['level']
                message = action['message']
                # Replace $pwd with $(pwd) for shell compatibility
                message = message.replace('$pwd', '$(pwd)')
                if level == 'info':
                    actions_code.append(f'    echo -e "${{BLUE}}ℹ️  {message}${{NC}}"')
                elif level == 'success':
                    actions_code.append(f'    echo -e "${{GREEN}}✅ {message}${{NC}}"')
                elif level == 'warning':
                    actions_code.append(f'    echo -e "${{YELLOW}}⚠️  {message}${{NC}}"')
                elif level == 'error':
                    actions_code.append(f'    echo -e "${{RED}}❌ {message}${{NC}}"')
                else:
                    actions_code.append(f'    echo "{message}"')
            elif action['type'] == 'prompt-user':
                question = action.get('question', 'user_input')
                actions_code.append(f'''    read -p "⚠️  {question} (y/N): " user_response
    if [[ "$user_response" =~ ^[Yy]$ ]]; then
        set_state_data "user_agreed" "true"
    else
        set_state_data "user_agreed" "false"
    fi''')
            else:
                # Custom action - call external script
                action_name = self.to_snake_case(action['type'])
                actions_code.append(f'''    # Execute custom action: {action['type']}
    if [[ -f "actions/{action_name}.sh" ]]; then
        source "actions/{action_name}.sh"
        result=$?
        if [[ $result -eq 0 ]]; then
            echo -e "${{GREEN}}✅ {action['type']} completed successfully${{NC}}"
        else
            echo -e "${{RED}}❌ {action['type']} failed${{NC}}"
            return 1
        fi
    else
        echo -e "${{YELLOW}}⚠️  Action script not found: actions/{action_name}.sh${{NC}}"
    fi''')
        
        return "\n".join(actions_code) if actions_code else "    # No actions to execute"
    
    def generate_shell_state_transitions(self, state: State) -> str:
        """Generate shell script transitions for a state"""
        if not state.transitions:
            return "    transition_to \"$CURRENT_STATE\""
        
        transition_code = []
        for transition in state.transitions:
            event_name = transition['event']
            target = transition['target']
            condition = transition.get('condition')
            
            if condition:
                # Convert condition to shell syntax
                shell_condition = self.convert_condition_to_shell(condition)
                transition_code.append(f"""    if {shell_condition}; then
        transition_to "{target}"
        return 0
    fi""")
            else:
                transition_code.append(f"""    transition_to "{target}"
    return 0""")
        
        return "\n".join(transition_code)
    
    def convert_condition_to_shell(self, condition: str) -> str:
        """Convert YAML condition to shell condition"""
        # Simple condition mapping - can be extended
        condition_map = {
            'beachball_installed': '[[ -n "$(command -v beachball)" ]]',
            'beachball_not_found': '[[ -z "$(command -v beachball)" ]]',
            'user_agreed': '[[ "$(get_state_data "user_agreed")" == "true" ]]',
            'user_declined': '[[ "$(get_state_data "user_agreed")" == "false" ]]'
        }
        
        return condition_map.get(condition, f'[[ "{condition}" == "true" ]]')
    
    def generate_shell_main_function(self) -> str:
        """Generate the main function for the shell script"""
        if not self.config:
            raise ValueError("Configuration not loaded")
            
        initial_state = self.get_initial_state()
        
        return f"""main() {{
    echo -e "${{BLUE}}🚀 Starting {self.config.name}${{NC}}"
    
    # Initialize state machine
    transition_to "{initial_state.id}"
    
    # Main state machine loop
    while [[ "$IS_COMPLETED" != "true" ]]; do
        case "$CURRENT_STATE" in
{self.generate_shell_state_switch()}
            *)
                echo -e "${{RED}}❌ Unknown state: $CURRENT_STATE${{NC}}"
                exit 1
                ;;
        esac
    done
    
    echo -e "${{GREEN}}🎉 State machine completed successfully!${{NC}}"
}}"""
    
    def generate_shell_state_switch(self) -> str:
        """Generate the state switch cases for the shell script"""
        switch_cases = []
        
        for state in self.states:
            function_name = f"execute_{self.to_snake_case(state.id)}_state"
            switch_cases.append(f"""            "{state.id}")
                {function_name}
                ;;""")
        
        return "\n".join(switch_cases)
    
    def generate_shell_helper_functions(self) -> str:
        """Generate helper functions for the shell script"""
        return """# Set state data
set_state_data() {
    local key="$1"
    local value="$2"
    STATE_DATA["$key"]="$value"
}

# Get state data
get_state_data() {
    local key="$1"
    echo "${STATE_DATA["$key"]:-}"
}

# Transition to a new state
transition_to() {
    local new_state="$1"
    echo -e "${BLUE}🔄 Transitioning to: $new_state${NC}"
    CURRENT_STATE="$new_state"
    IS_COMPLETED=false
}

# Complete the state machine
complete() {
    IS_COMPLETED=true
    echo -e "${GREEN}✅ State machine completed${NC}"
}

# Handle errors
handle_error() {
    local message="$1"
    ERROR_MESSAGE="$message"
    echo -e "${RED}❌ Error: $message${NC}"
    IS_COMPLETED=true
    exit 1
}"""
    
    def generate_action_scripts(self) -> None:
        """Generate individual action scripts"""
        if not self.config:
            raise ValueError("Configuration not loaded")
        
        actions_dir = self.output_dir / "actions"
        actions_dir.mkdir(exist_ok=True)
        
        for action in self.custom_actions:
            if not action:
                continue
            script_name = f"{self.to_snake_case(action.name)}.sh"
            script_path = actions_dir / script_name
            
            script_content = f"""#!/bin/bash
# {action.name} - {action.description}
# Generated action script for {self.config.name}

set -euo pipefail

# Colors for output
GREEN='\\033[0;32m'
RED='\\033[0;31m'
NC='\\033[0m'

echo "🔧 Executing {action.name}..."

# TODO: Implement your custom logic here
# This is a placeholder implementation

# Example implementation:
# case "{action.name}" in
#     "verify-beachball-installation")
#         if command -v beachball >/dev/null 2>&1; then
#             echo -e "${{GREEN}}✅ Beachball is installed${{NC}}"
#             exit 0
#         else
#             echo -e "${{RED}}❌ Beachball is not installed${{NC}}"
#             exit 1
#         fi
#         ;;
#     "install-beachball-package")
#         npm install -g beachball
#         echo -e "${{GREEN}}✅ Beachball installed successfully${{NC}}"
#         exit 0
#         ;;
#     "verify-changelog-files")
#         if [[ -f "CHANGELOG.md" ]]; then
#             echo -e "${{GREEN}}✅ Changelog files verified${{NC}}"
#             exit 0
#         else
#             echo -e "${{RED}}❌ Changelog files not found${{NC}}"
#             exit 1
#         fi
#         ;;
#     "prompt-user")
#         read -p "User input: " user_input
#         echo "$user_input"
#         exit 0
#         ;;
#     *)
#         echo -e "${{RED}}❌ Unknown action: {action.name}${{NC}}"
#         exit 1
#         ;;
# esac

echo -e "${{GREEN}}✅ {action.name} completed successfully${{NC}}"
exit 0"""
            
            with open(script_path, 'w', encoding='utf-8') as f:
                f.write(script_content)
            
            # Make script executable
            os.chmod(script_path, 0o755)
            print(f"✅ Generated action script: {script_path}")
    
    def generate(self) -> None:
        """Generate all the code files"""
        self.load_yaml()
        
        if not self.config:
            raise ValueError("Configuration not loaded")
        
        # Create output directory
        self.output_dir.mkdir(parents=True, exist_ok=True)
        
        # Generate C# code
        if not self.generate_shell:
            # Generate main workflow file
            main_content = f"""using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace {self.config.namespace}
{{
    {self.generate_workflow_result()}
    
    {self.generate_base_class()}
    
    {self.generate_interfaces()}
    
    {self.generate_state_machine_class()}
}}"""
            
            main_file = self.output_dir / f"{self.config.name}.cs"
            with open(main_file, 'w', encoding='utf-8') as f:
                f.write(main_content)
            
            print(f"✅ Generated main workflow: {main_file}")
            
            # Generate tests if requested
            if self.config.generate_tests:
                test_content = self.generate_tests()
                test_file = self.output_dir / f"{self.config.name}Tests.cs"
                with open(test_file, 'w', encoding='utf-8') as f:
                    f.write(test_content)
                print(f"✅ Generated tests: {test_file}")
        
        # Generate shell script
        if self.generate_shell:
            shell_content = self.generate_shell_script()
            shell_file = self.output_dir / f"{self.to_snake_case(self.config.name)}.sh"
            with open(shell_file, 'w', encoding='utf-8') as f:
                f.write(shell_content)
            
            # Make script executable
            os.chmod(shell_file, 0o755)
            print(f"✅ Generated shell script: {shell_file}")
            
            # Generate action scripts
            self.generate_action_scripts()
        
        # Generate README
        readme_content = f"""# {self.config.name}

Generated state machine for {self.config.description}

## Usage

"""
        
        if not self.generate_shell:
            readme_content += f"""```csharp
var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<{self.config.name}>();
var actions = new {self.config.name}Actions(); // Implement I{self.config.name}Actions
var workflow = new {self.config.name}(logger, actions);

var result = await workflow.StartAsync();
```"""
        else:
            readme_content += f"""```bash
# Make the script executable
chmod +x {self.to_snake_case(self.config.name)}.sh

# Run the state machine
./{self.to_snake_case(self.config.name)}.sh

# Or with custom parameters
./{self.to_snake_case(self.config.name)}.sh --debug --verbose
```"""
        
        readme_content += f"""

## States

{chr(10).join([f"- **{state.name}**: {state.description}" for state in self.states])}

## Events

{chr(10).join([f"- **{event.name}**: {event.description}" for event in self.events])}

## Custom Actions

{chr(10).join([f"- **{action.name}**: {action.description} (returns {action.return_type})" for action in self.custom_actions])}

"""
        
        if self.generate_shell:
            readme_content += f"""## Action Scripts

The following action scripts are generated in the `actions/` directory:

{chr(10).join([f"- `actions/{self.to_snake_case(action.name)}.sh` - {action.description}" for action in self.custom_actions])}

You can customize these scripts to implement your specific logic.
"""
        
        readme_content += f"""
Generated on: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}
"""
        
        readme_file = self.output_dir / "README.md"
        with open(readme_file, 'w', encoding='utf-8') as f:
            f.write(readme_content)
        print(f"✅ Generated README: {readme_file}")
        
        print(f"\n🎉 State machine generation completed!")
        print(f"📁 Output directory: {self.output_dir.absolute()}")


def main():
    """Main entry point"""
    parser = argparse.ArgumentParser(description='Generate state machine code from YAML')
    parser.add_argument('yaml_file', help='Path to the YAML state machine definition')
    parser.add_argument('output_dir', nargs='?', default='generated', help='Output directory (default: generated)')
    parser.add_argument('--shell', action='store_true', help='Generate shell script instead of C# code')
    
    args = parser.parse_args()
    
    if not os.path.exists(args.yaml_file):
        print(f"❌ YAML file not found: {args.yaml_file}")
        sys.exit(1)
    
    print(f"🚀 Generating state machine from: {args.yaml_file}")
    generator = StateMachineGenerator(args.yaml_file, args.output_dir, args.shell)
    generator.generate()


if __name__ == "__main__":
    main() 