#!/bin/bash

# Exit on any error
set -e

echo "🔄 Restoring packages..."

# Run npm install at solution root
echo "📦 Running npm install..."
npm install

# Run dotnet restore at solution root
echo "🔧 Running dotnet restore..."
dotnet restore

# Run flutter pub get in the flutter app
echo "📱 Running flutter pub get..."
flutter pub get --directory apps/frontend/flutter/*

echo "✅ Package restoration completed successfully!"
