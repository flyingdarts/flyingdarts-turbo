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

# Directories to search within
search_dirs="
	apps/backend/dotnet
	apps/backend/rust
	apps/frontend/flutter
	apps/frontend/angular
	apps/tools/dotnet
	packages/backend/dotnet
	packages/backend/rust
	packages/frontend/flutter
	packages/tools/dotnet"

# folders to remove
folders_to_remove="
	node_modules
	dist
	.turbo
	.angular
	bin
	obj
	.dart_tool
	target
  build"

files_to_remove="
	package-lock.json
	pubspec.lock
	Podfile.lock
	pubspec_overrides.yaml
	.flutter-plugins-dependencies
	.flutter-plugins"

### Search once in root for all files and folders to remove
for file in $files_to_remove; do
	remove_file "$file"
done

for dir in $folders_to_remove; do
	remove_dir "$dir"
done

### Recursively search workspaces for files and folders to remove
for search_dir in $search_dirs; do
	if [ -d "$search_dir" ]; then
		echo "🔍 Searching in $search_dir..."

		# Remove files recursively
		for file in $files_to_remove; do
			find "$search_dir" -name "$file" -type f 2>/dev/null | while read -r found_file; do
				if [ -n "$found_file" ]; then
					rm -f "$found_file"
					echo "🗑️  $found_file has been removed."
				fi
			done
		done

		# Remove directories recursively
		for dir in $folders_to_remove; do
			find "$search_dir" -name "$dir" -type d 2>/dev/null | while read -r found_dir; do
				if [ -n "$found_dir" ]; then
					rm -rf "$found_dir"
					echo "🗑️  $found_dir has been removed."
				fi
			done
		done
	fi
done

# Clear npm cache
echo "🧹 Clearing npm cache..."
npm cache clean --force >/dev/null 2>&1

# Clear NuGet cache
echo "🧹 Clearing NuGet cache..."
dotnet nuget locals all --clear >/dev/null 2>&1

echo "✨ All cleaned up!"
