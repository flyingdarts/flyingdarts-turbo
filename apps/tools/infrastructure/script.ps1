# Display environment options
Write-Host "Select environment to deploy:"
Write-Host "1. Development"
Write-Host "2. Production"

# Prompt user to select environment
$userChoice = Read-Host "Enter the number corresponding to the environment:"

# Validate user input
if ($userChoice -eq "1") {
    $environment = "Development"
} elseif ($userChoice -eq "2") {
    $environment = "Production"
} else {
    Write-Host "Invalid choice. Please enter either 1 or 2."
    Exit
}

# Form the deploy command based on the selected environment
$deployCommand = "cdk deploy FD-Stack-$environment"

# Display the command for confirmation
Write-Host "Command to be executed: $deployCommand"

# Prompt user for confirmation
$confirmation = Read-Host "Do you want to proceed with the deployment? (Y/N):"

# Check user confirmation
if ($confirmation -eq "Y" -or $confirmation -eq "y") {
    # Execute the deploy command
    Invoke-Expression $deployCommand
} else {
    Write-Host "Deployment cancelled."
}
