# Get the current working directory
$currentDirectory = Get-Location

# Display the current working directory
Write-Host "Current Working Directory: $currentDirectory"

# Array of project folders for signaling/user
$signalingUserFolders = @(
    "Flyingdarts.Backend.Signalling.OnConnect",
    "Flyingdarts.Backend.Signalling.OnDefault",
    "Flyingdarts.Backend.Signalling.OnDisconnect",
    "Flyingdarts.Backend.User.Profile.Create",
    "Flyingdarts.Backend.User.Profile.Get",
    "Flyingdarts.Backend.User.Profile.Create",
    "Flyingdarts.Backend.User.Profile.VerifyEmail",
    "Flyingdarts.Backend.User.UpdateConnectionId"
)

# Loop through the signaling/user folders
foreach ($folder in $signalingUserFolders) {
    $fullPath = Join-Path -Path $currentDirectory -ChildPath $folder
    Write-Host "Processing folder: $fullPath"

    # Change to the folder for execution
    Set-Location -Path $fullPath
    
    # Your code for these folders can go here
    git add .
    git commit -m "Update package references"
    git push -u origin main

    # Return to the current directory
    Set-Location -Path $currentDirectory
}