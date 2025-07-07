#!/bin/bash

# Generate Dyte API Client using Microsoft Kiota with FIXED specification
# Usage: ./scripts/generate-charp-dyte-client.sh

OPENAPI_SPEC_PATH=${1:-"specs/dyte-api-spec-fixed.yaml"}
OUTPUT_PATH=${2:-"packages/Flyingdarts.Meetings.Service/Generated/Dyte"}
NAMESPACE=${3:-"Flyingdarts.Meetings.Service.Generated.Dyte"}
CLASS_NAME=${4:-"DyteApiClient"}

echo "Generating Dyte API Client using Microsoft Kiota with FIXED specification..."

# Create output directory if it doesn't exist
mkdir -p "$OUTPUT_PATH"

# Install Kiota if not already installed
if ! command -v kiota &>/dev/null; then
    echo "Installing Microsoft Kiota..."
    dotnet tool install -g Microsoft.OpenApi.Kiota
fi

# Clean output directory if it exists
if [ -d "$OUTPUT_PATH" ]; then
    echo "Cleaning output directory..."
    rm -rf "$OUTPUT_PATH"/*
fi

# Generate the client
echo "Generating client from FIXED OpenAPI spec..."
kiota generate \
    --openapi "$OPENAPI_SPEC_PATH" \
    --language csharp \
    --output "$OUTPUT_PATH" \
    --class-name "$CLASS_NAME" \
    --namespace-name "$NAMESPACE" \
    --clean-output \
    --serializer Microsoft.Kiota.Serialization.Json.JsonSerializationWriterFactory \
    --deserializer Microsoft.Kiota.Serialization.Json.JsonParseNodeFactory \
    --backing-store

echo "Client generated successfully at: $OUTPUT_PATH"
echo "This should now have proper Meeting and Participant objects in response data!"
echo "Don't forget to add the generated files to your project!"
echo "You may need to add the following NuGet packages to your project:"
echo "  - Microsoft.Kiota.Http.HttpClientLibrary"
echo "  - Microsoft.Kiota.Serialization.Json"
echo "  - Microsoft.Kiota.Abstractions"
