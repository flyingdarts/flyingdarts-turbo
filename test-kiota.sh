#!/bin/bash

echo "Testing Kiota generation..."

# Test with a simple command
kiota generate \
    --openapi specs/dyte-api-spec.yaml \
    --language csharp \
    --output test-output \
    --class-name TestClient \
    --namespace-name Test \
    --clean-output \
    --serializer SystemTextJson \
    --deserializer SystemTextJson \
    --backing-store

echo "Test completed"
