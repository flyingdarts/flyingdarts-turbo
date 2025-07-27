# Display environment options
Write-Host "Select stack to deploy:"
Write-Host "1. FD-Stack-Development"
Write-Host "2. FD-Stack-Staging"
Write-Host "3. FD-Stack-Production"
Write-Host "4. Auth-Stack"


# Prompt user to select environment
$userChoice = Read-Host "Enter the number corresponding to the environment:"

# Validate user input
if ($userChoice -eq "1") {
    $deployCommand = "FD-Stack-Development"
} elseif ($userChoice -eq "2") {
    $deployCommand = "FD-Stack-Staging"
} elseif ($userChoice -eq "3") {
    $deployCommand = "FD-Stack-Production"
} elseif ($userChoice -eq "3") {
    $deployCommand = "Auth-Stack"
} else {
    Write-Host "Invalid choice. Please enter either 1, 2, 3 or 4."
    Exit
}

# Form the deploy command based on the selected environment
$deployCommand = "cdk deploy $deployCommand"

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
