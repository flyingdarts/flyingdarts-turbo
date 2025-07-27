#!/bin/bash

# 🎯 Fix Flutter Package Names Script
# This script scans for package.json files in packages/frontend/flutter
# and updates corresponding pubspec.yaml files with the package.json name

set -e # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

# Global variables
DRY_RUN=false

# Function to print colored output
print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_dry_run() {
    echo -e "${PURPLE}🔍 [DRY RUN] $1${NC}"
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [OPTIONS]"
    echo
    echo "Options:"
    echo "  --dry-run    Show what changes would be made without applying them"
    echo "  -h, --help   Show this help message"
    echo
    echo "Examples:"
    echo "  $0                    # Apply changes with confirmation"
    echo "  $0 --dry-run          # Show what would be changed without applying"
    echo "  $0 -h                 # Show help"
}

# Function to parse command line arguments
parse_args() {
    while [[ $# -gt 0 ]]; do
        case $1 in
        --dry-run)
            DRY_RUN=true
            shift
            ;;
        -h | --help)
            show_usage
            exit 0
            ;;
        *)
            print_error "Unknown option: $1"
            show_usage
            exit 1
            ;;
        esac
    done
}

# Function to prompt user for confirmation
confirm_action() {
    local message="$1"
    echo -e "${YELLOW}🤔 $message${NC}"
    read -p "Do you want to proceed? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        print_info "Operation cancelled by user."
        exit 0
    fi
}

# Function to convert package.json name to pubspec.yaml format
convert_name() {
    local package_name="$1"
    # Convert dots to underscores for pubspec.yaml (Flutter naming convention)
    echo "$package_name" | sed 's/\./_/g'
}

# Function to get the simple package name (last part after dots/underscores)
get_simple_package_name() {
    local package_name="$1"
    # Get the last part after dots or underscores, or the whole name if no separators
    if [[ "$package_name" == *"."* || "$package_name" == *"_"* ]]; then
        echo "$package_name" | sed 's/.*[._]//'
    else
        echo "$package_name"
    fi
}

# Function to rename package folder
rename_package_folder() {
    local old_path="$1"
    local new_name="$2"
    local parent_dir=$(dirname "$old_path")
    local new_path="$parent_dir/$new_name"
    local current_folder_name=$(basename "$old_path")

    if [[ "$current_folder_name" != "$new_name" ]]; then
        print_info "  📁 Renaming folder: $current_folder_name -> $new_name"
        mv "$old_path" "$new_path"
        print_success "  ✅ Folder renamed successfully"
        return 0
    else
        print_info "  ✅ Folder name already correct: $current_folder_name"
        return 0
    fi
}

# Function to extract package name from package.json
extract_package_name() {
    local package_json="$1"
    # Use jq if available, otherwise fall back to grep/sed
    if command -v jq &>/dev/null; then
        jq -r '.name' "$package_json" 2>/dev/null || echo ""
    else
        grep '"name"' "$package_json" | head -1 | sed 's/.*"name":\s*"\([^"]*\)".*/\1/'
    fi
}

# Function to extract current name from pubspec.yaml
extract_pubspec_name() {
    local pubspec_yaml="$1"
    local name=$(grep '^name:' "$pubspec_yaml" | head -1 | sed 's/name:\s*\([^[:space:]]*\).*/\1/')
    # Return empty string if name is empty or just whitespace
    if [[ -z "$name" || "$name" == "" ]]; then
        echo ""
    else
        echo "$name"
    fi
}

# Function to validate and suggest updates for all relative path dependencies in pubspec.yaml files
validate_relative_import_paths() {
    print_info "🔍 Scanning for relative import paths in pubspec.yaml files..."

    # Build a map of old_folder_name -> new_folder_name for all packages
    declare -A folder_map
    local flutter_dir="packages/frontend/flutter"
    local package_json_files=($(find "$flutter_dir" -name "package.json" -type f))
    for package_json in "${package_json_files[@]}"; do
        local package_dir=$(dirname "$package_json")
        local old_folder_name=$(basename "$package_dir")
        local package_name=$(extract_package_name "$package_json")
        if [[ -n "$package_name" ]]; then
            local new_folder_name=$(convert_name "$package_name")
            folder_map["$old_folder_name"]="$new_folder_name"
        fi
    done

    # Now scan all pubspec.yaml files for relative path dependencies
    local pubspec_files=($(find "$flutter_dir" -name "pubspec.yaml" -type f))
    for pubspec in "${pubspec_files[@]}"; do
        local dir_of_pubspec=$(dirname "$pubspec")
        while IFS= read -r line; do
            if [[ "$line" =~ path:\ (.*) ]]; then
                local rel_path="${BASH_REMATCH[1]}"
                # Only consider relative paths
                if [[ "$rel_path" == ./* ]]; then
                    local dep_folder=$(basename "$rel_path")
                    local dep_parent=$(dirname "$rel_path")
                    local new_folder="${folder_map[$dep_folder]}"
                    if [[ -n "$new_folder" && "$dep_folder" != "$new_folder" ]]; then
                        print_warning "Would update $pubspec:"
                        print_warning "  path: $rel_path -> path: $dep_parent/$new_folder"
                    fi
                fi
            fi
        done <"$pubspec"
    done

    print_info "Validation complete. No files were changed."
}

# Main script logic
main() {
    # Parse command line arguments
    parse_args "$@"

    if [[ "$DRY_RUN" == true ]]; then
        print_dry_run "DRY RUN MODE - No changes will be applied"
        echo
    fi

    print_info "🔍 Scanning for package.json files in packages/frontend/flutter..."

    # Find all package.json files in packages/frontend/flutter
    local flutter_dir="packages/frontend/flutter"

    if [[ ! -d "$flutter_dir" ]]; then
        print_error "Directory $flutter_dir not found!"
        exit 1
    fi

    # Find all package.json files recursively
    local package_json_files=($(find "$flutter_dir" -name "package.json" -type f))

    if [[ ${#package_json_files[@]} -eq 0 ]]; then
        print_info "No package.json files found in $flutter_dir"
        exit 0
    fi

    print_success "Found ${#package_json_files[@]} package.json file(s)"

    local changes_to_make=()
    local package_details=()
    local folder_renames=()
    local new_pubspec_names=()

    # Process each package.json file
    for package_json in "${package_json_files[@]}"; do
        print_info "📦 Processing: $package_json"

        # Extract package name from package.json
        local package_name=$(extract_package_name "$package_json")

        if [[ -z "$package_name" ]]; then
            print_warning "No 'name' field found in $package_json, skipping..."
            continue
        fi

        print_info "  📋 Package name from package.json: $package_name"

        # Find corresponding pubspec.yaml
        local package_dir=$(dirname "$package_json")
        local pubspec_yaml="$package_dir/pubspec.yaml"

        if [[ ! -f "$pubspec_yaml" ]]; then
            print_warning "  ⚠️  No pubspec.yaml found in $package_dir"
            continue
        fi

        # Extract current pubspec.yaml name
        local current_pubspec_name=$(extract_pubspec_name "$pubspec_yaml")

        # Debug output
        print_info "  🔍 Raw pubspec name: '$current_pubspec_name'"

        # Convert package.json name to pubspec format
        local new_pubspec_name=$(convert_name "$package_name")

        print_info "  📋 Current pubspec.yaml name: $current_pubspec_name"
        print_info "  📋 New pubspec.yaml name: $new_pubspec_name"

        # Get folder name (should match pubspec name)
        local folder_name="$new_pubspec_name"
        local current_folder_name=$(basename "$package_dir")

        print_info "  📁 Current folder name: $current_folder_name"
        print_info "  📁 New folder name: $folder_name"

        # Check if we need to update (either empty name or different name)
        local needs_pubspec_update=false
        local needs_folder_rename=false

        if [[ -z "$current_pubspec_name" || "$current_pubspec_name" != "$new_pubspec_name" ]]; then
            needs_pubspec_update=true
        fi

        if [[ "$current_folder_name" != "$folder_name" ]]; then
            needs_folder_rename=true
        fi

        if [[ "$needs_pubspec_update" == true || "$needs_folder_rename" == true ]]; then
            local change_description=""

            if [[ "$needs_pubspec_update" == true ]]; then
                if [[ -z "$current_pubspec_name" ]]; then
                    change_description="(empty) -> $new_pubspec_name"
                else
                    change_description="$current_pubspec_name -> $new_pubspec_name"
                fi
            fi

            if [[ "$needs_folder_rename" == true ]]; then
                if [[ -n "$change_description" ]]; then
                    change_description="$change_description + folder: $current_folder_name -> $folder_name"
                else
                    change_description="folder: $current_folder_name -> $folder_name"
                fi
            fi

            change_description="$change_description (in $pubspec_yaml)"

            changes_to_make+=("$pubspec_yaml")
            package_details+=("$change_description")

            # Store new pubspec name if update is needed
            if [[ "$needs_pubspec_update" == true ]]; then
                new_pubspec_names+=("$new_pubspec_name")
            else
                new_pubspec_names+=("") # Empty string for no update
            fi

            # Store folder rename info
            if [[ "$needs_folder_rename" == true ]]; then
                folder_renames+=("$package_dir:$folder_name")
            fi

            if [[ "$needs_pubspec_update" == true ]]; then
                print_info "  📝 Will update pubspec: $current_pubspec_name -> $new_pubspec_name"
            fi
            if [[ "$needs_folder_rename" == true ]]; then
                print_info "  📁 Will rename folder: $current_folder_name -> $folder_name"
            fi
        else
            print_info "  ✅ Already correct: $new_pubspec_name"
        fi
    done

    # If there are changes to make
    if [[ ${#changes_to_make[@]} -gt 0 ]]; then
        echo
        if [[ "$DRY_RUN" == true ]]; then
            print_dry_run "The following changes would be made:"
        else
            print_warning "The following changes will be made:"
        fi

        for detail in "${package_details[@]}"; do
            echo "  • $detail"
        done
        echo

        if [[ "$DRY_RUN" == true ]]; then
            print_dry_run "Dry run completed. Run without --dry-run to apply changes."
        else
            confirm_action "Are you sure you want to update these pubspec.yaml files and rename folders?"

            # Apply changes
            print_info "🔄 Applying changes..."

            # First, update pubspec.yaml files
            for i in "${!changes_to_make[@]}"; do
                local pubspec_yaml="${changes_to_make[$i]}"
                local new_name="${new_pubspec_names[$i]}"

                # Only update if we have a new name
                if [[ -n "$new_name" ]]; then
                    print_info "Updating $pubspec_yaml with name: $new_name"

                    # Update the name in pubspec.yaml
                    if [[ "$OSTYPE" == "darwin"* ]]; then
                        # macOS
                        sed -i '' "s/^name:.*/name: $new_name/" "$pubspec_yaml"
                    else
                        # Linux
                        sed -i "s/^name:.*/name: $new_name/" "$pubspec_yaml"
                    fi

                    print_success "Updated $pubspec_yaml"
                fi
            done

            # Then, rename folders
            if [[ ${#folder_renames[@]} -gt 0 ]]; then
                print_info "📁 Renaming package folders..."
                for folder_rename in "${folder_renames[@]}"; do
                    local old_path=$(echo "$folder_rename" | cut -d':' -f1)
                    local new_name=$(echo "$folder_rename" | cut -d':' -f2)

                    if [[ -d "$old_path" ]]; then
                        rename_package_folder "$old_path" "$new_name"
                    else
                        print_warning "  ⚠️  Folder not found: $old_path"
                    fi
                done
            fi

            print_success "🎉 All changes completed successfully!"
        fi
    else
        print_info "✨ No changes needed - all package names are already correct!"
    fi
}

# Run the main function
main "$@"
