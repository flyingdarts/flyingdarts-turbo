
# Ask user how many runners they want to create
echo ""
read -p "How many action runners do you want to start? " num_runners

# Validate input is a positive integer
if ! [[ "$num_runners" =~ ^[1-9][0-9]*$ ]]; then
    echo "❌ Error: Please enter a valid positive number"
    exit 1
fi

# Check if .runners directory exists
if [ ! -d ".runners" ]; then
    echo "❌ Error: .runners directory not found"
    exit 1
fi

echo "🚀 Starting $num_runners action runners..."

# Iterate over the specified number of runners
for ((i=1; i<=num_runners; i++)); do
    runner_dir=".runners/action-runner-$i"
    
    # Check if the runner directory exists
    if [ ! -d "$runner_dir" ]; then
        echo "⚠️  Warning: $runner_dir not found, skipping..."
        continue
    fi
    
    # Check if run.sh exists in the runner directory
    if [ ! -f "$runner_dir/run.sh" ]; then
        echo "⚠️  Warning: run.sh not found in $runner_dir, skipping..."
        continue
    fi
    
    echo "🔄 Starting action-runner-$i..."
    
    # Change to the runner directory and execute run.sh
    cd "$runner_dir"
    ./run.sh &
    cd - > /dev/null
    
    echo "✅ Started action-runner-$i (PID: $!)"
done

echo "🎉 All runners started successfully!"

