#!/bin/bash

# Check if .runners directory exists at workspace root
if [ ! -d ".runners" ]; then
    echo "Creating .runners directory at workspace root..."
    mkdir -p .runners
    echo "✅ .runners directory created successfully"
else
    echo "✅ .runners directory already exists"
fi

# Ask user how many runners they want to create
echo ""
read -p "How many action runners do you want to create? " num_runners

# Validate input is a positive integer
if ! [[ "$num_runners" =~ ^[1-9][0-9]*$ ]]; then
    echo "❌ Error: Please enter a valid positive number"
    exit 1
fi

echo ""
echo "Creating $num_runners action runner folders..."

# Create action-runner folders
for i in $(seq 1 $num_runners); do
    runner_folder=".runners/action-runner-$i"
    if [ ! -d "$runner_folder" ]; then
        mkdir -p "$runner_folder"
        echo "✅ Created: $runner_folder"
    else
        echo "⚠️  Already exists: $runner_folder"
    fi
done

# Download the GitHub Actions runner tar file
echo ""
echo "📥 Downloading GitHub Actions runner..."
tar_file=".runners/actions-runner-osx-arm64-2.326.0.tar.gz"
if [ ! -f "$tar_file" ]; then
    curl -o "$tar_file" -L https://github.com/actions/runner/releases/download/v2.326.0/actions-runner-osx-arm64-2.326.0.tar.gz
    if [ $? -eq 0 ]; then
        echo "✅ Downloaded: $tar_file"
    else
        echo "❌ Error: Failed to download the runner tar file"
        exit 1
    fi
else
    echo "✅ Tar file already exists: $tar_file"
fi

# Extract the tar file into each action-runner folder
echo ""
echo "📦 Extracting runner files into each action-runner folder..."
for i in $(seq 1 $num_runners); do
    runner_folder=".runners/action-runner-$i"
    echo "Processing: $runner_folder"
    
    # Clear the folder contents (force and silent)
    rm -rf "$runner_folder"/* 2>/dev/null
    echo "🧹 Cleared existing contents"
    
    # Extract the tar file into the runner folder
    tar -xzf "$tar_file" -C "$runner_folder" >/dev/null 2>&1
    if [ $? -eq 0 ]; then
        echo "✅ Extracted successfully into: $runner_folder"
    else
        echo "❌ Error: Failed to extract into: $runner_folder"
        exit 1
    fi
done

# Configure each runner with a token
echo ""
echo "🔧 Configuring each runner with GitHub token..."
for i in $(seq 1 $num_runners); do
    runner_folder=".runners/action-runner-$i"
    echo ""
    echo "--- Configuring action-runner-$i ---"
    
    # Ask for token for this specific runner
    read -p "Enter token for action-runner-$i: " token
    
    # Validate token is not empty
    if [ -z "$token" ]; then
        echo "❌ Error: Token cannot be empty for action-runner-$i"
        exit 1
    fi
    
    # Configure the runner
    echo "Configuring runner..."
    cd "$runner_folder"
    ./config.sh --url https://github.com/flyingdarts/flyingdarts-turbo --token "$token"
    if [ $? -eq 0 ]; then
        echo "✅ Successfully configured action-runner-$i"
    else
        echo "❌ Error: Failed to configure action-runner-$i"
        exit 1
    fi
    cd - >/dev/null 2>&1
done

echo ""
echo "🎉 Setup complete! Created and configured $num_runners action runner folders in .runners/"
