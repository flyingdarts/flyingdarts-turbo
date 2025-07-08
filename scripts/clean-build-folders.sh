#!/bin/sh

remove_dir() {
	dir_name="$1"
	if [ -d "$dir_name" ]; then
		rm -rf "$dir_name"
		echo "$dir_name has been removed."
	fi
}

remove_file() {
	file_name="$1"
	if [ -f "$file_name" ]; then
		rm -rf "$file_name"
		echo "$file_name has been removed."
	fi
}

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
for dir in $search_dirs; do
	for pattern in $dirs_patterns; do
		find "$dir" -type d -name "$pattern" -exec rm -rf {} + -exec echo "{} has been removed." \;
	done
done

# Find and remove all pubspec.lock files recursively, with user confirmation
found_files=""
for dir in $search_dirs; do
	if [ -d "$dir" ]; then
		files=$(find "$dir" -type f -name "pubspec.lock")
		if [ -n "$files" ]; then
			found_files="$found_files\n$files"
		fi
	fi
done

if [ -n "$found_files" ]; then
	echo "🧹 Found the following pubspec.lock files to remove:"
	echo "$found_files" | sed '/^$/d'
	echo "Are you sure you want to delete ALL these pubspec.lock files? (y/N) 🚨 "
	read confirm
	case "$confirm" in
	y | Y)
		echo "$found_files" | sed '/^$/d' | while IFS= read file; do
			rm -f "$file"
			echo "$file has been removed."
		done
		;;
	*)
		echo "Aborted removal of pubspec.lock files."
		;;
	esac
else
	echo "No pubspec.lock files found to remove."
fi

# Clear npm cache
echo "Clearing npm cache..."
npm cache clean --force

# Clear NuGet cache
echo "Clearing NuGet cache..."
dotnet nuget locals all --clear

echo "All cleaned up!"
