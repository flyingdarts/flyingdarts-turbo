#!/bin/sh

remove_dir() {
	dir_name="$1"
	if [ -d "$dir_name" ]; then
		rm -rf "$dir_name"
		echo "🗑️  $dir_name has been removed."
	fi
}

remove_file() {
	file_name="$1"
	if [ -f "$file_name" ]; then
		rm -rf "$file_name"
		echo "🗑️  $file_name has been removed."
	fi
}

echo "🧹 Starting cleanup process..."

# Remove top-level items
remove_dir "node_modules"
remove_dir ".turbo"
remove_file "package-lock.json"
remove_file "pubspec.lock"

# Directories to search within
search_dirs="
	apps/backend/dotnet 
	apps/frontend/flutter 
	apps/tools/dotnet
	packages/backend/dotnet 
	packages/frontend/flutter
	packages/tools/dotnet"

# Patterns to find and remove
dirs_patterns="node_modules dist .turbo .angular bin obj"

# Loop through search dirs & patterns
# Remove each find of patterns
echo "🔍 Searching for build directories to clean..."
for dir in $search_dirs; do
	for pattern in $dirs_patterns; do
		find "$dir" -type d -name "$pattern" -exec rm -rf {} + 2>/dev/null
	done
done

# Find and remove all pubspec.lock files recursively (no prompt)
echo "📦 Removing pubspec.lock files..."
for dir in $search_dirs; do
	if [ -d "$dir" ]; then
		find "$dir" -type f -name "pubspec.lock" -exec rm -f {} + 2>/dev/null
	fi
done

# Clear npm cache
echo "🧹 Clearing npm cache..."
npm cache clean --force >/dev/null 2>&1

# Clear NuGet cache
echo "🧹 Clearing NuGet cache..."
dotnet nuget locals all --clear >/dev/null 2>&1

echo "✨ All cleaned up!"
