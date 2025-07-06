#!/bin/bash

remove_dir() {
	local dir_name="$1"
	if [ -d "$dir_name" ]; then
		rm -rf "$dir_name"
		echo "$dir_name has been removed."
	fi
}

remove_file() {
	local file_name="$1"
	if [ -f "$file_name" ]; then
		rm -rf "$file_name"
		echo "$file_name has been removed."
	fi
}

# Remove top-level items
remove_dir "node_modules"
remove_dir ".turbo"
remove_file "package-lock.json"

# Directories to search within
search_dirs=("apps" "packages")

# Patterns to find and remove
patterns=("node_modules" "dist" ".turbo" ".angular" "bin" "obj")

# Loop through search dirs & patterns
# Remove each find of patterns
for dir in "${search_dirs[@]}"; do
	for pattern in "${patterns[@]}"; do
		find "$dir" -type d -name "$pattern" -exec rm -rf {} + -exec echo "{} has been removed." \;
	done
done

# Clear npm cache
echo "Clearing npm cache..."
npm cache clean --force

# Clear NuGet cache
echo "Clearing NuGet cache..."
dotnet nuget locals all --clear

echo "All cleaned up!"
