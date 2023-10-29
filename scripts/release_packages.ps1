# Get the current working directory
$currentDirectory = Get-Location

# Display the current working directory
Write-Host "Current Working Directory: $currentDirectory"

# Array of project folders where you need to update the .sproj file
$packagesToUpdateSproj = @(
    "Flyingdarts.Shared",
    "Flyingdarts.Persistence",
    "Flyingdarts.Backend.Shared"
)
# Increment minor version function
function IncrementMinorVersion($versionPrefix) {
    $versionParts = $versionPrefix -split '\.'
    if ($versionParts.Length -eq 3) {
        $major = [int]$versionParts[0]
        $minor = [int]$versionParts[1]
        $patch = [int]$versionParts[2]
        $patch++
        return "$major.$minor.$patch"
    }
    else {
        Write-Host "Invalid version format: $versionPrefix"
        return $versionPrefix
    }
}

$sharedVersion
$persistenceVersion
$backendSharedVersion

# Loop through the packages that need .sproj updates
foreach ($folder in $packagesToUpdateSproj) {
    $fullPath = Join-Path -Path $currentDirectory -ChildPath $folder
    Write-Host "Processing folder $fullPath"
    # Get .csproj files in the folder
    $csprojFiles = Get-ChildItem -Path $fullPath -Filter "*.csproj"
    foreach ($csprojFile in $csprojFiles) {
        Write-Host "Extracting VersionPrefix from $($csprojFile.Name):"
        
        # Load the .csproj file as an XML document
        $xml = [xml](Get-Content -Path $csprojFile.FullName)

        # Extract the VersionPrefix value
        $versionPrefix = $xml.Project.PropertyGroup.VersionPrefix
        Write-Host "VersionPrefix: $versionPrefix"
        # Increment the minor version
        $newVersionPrefix = IncrementMinorVersion $versionPrefix

        # Update the VersionPrefix value
        $xml.Project.PropertyGroup.VersionPrefix = $newVersionPrefix

        # Save the updated .csproj file
        $xml.Save($csprojFile.FullName)
        
        Write-Host "New versionPrefix: $newVersionPrefix"

        # Check if it's Flyingdarts.Persistence.csproj
        if ($csprojFile.Name -eq "Flyingdarts.Shared.csproj") {
            $sharedVersion = $newVersionPrefix
        }

        # Check if it's Flyingdarts.Persistence.csproj
        if ($csprojFile.Name -eq "Flyingdarts.Persistence.csproj") {
            $persistenceVersion = $newVersionPrefix
        }

        # Check if it's Flyingdarts.Persistence.csproj
        if ($csprojFile.Name -eq "Flyingdarts.Backend.Shared.csproj") {
            $backendSharedVersion = $newVersionPrefix
        }
    }

    Write-Host "Updating .csproj file for $folder"
    
    # Your code to update the .sproj file for these packages can go here
}
Write-Host "Version Shared: $sharedVersion Version Persistence: $persistenceVersion Version BackendShared: $backendSharedVersion"


# Loop through the packages that need .sproj updates
foreach ($folder in $packagesToUpdateSproj) {
    $fullPath = Join-Path -Path $currentDirectory -ChildPath $folder
    Write-Host "Processing folder $fullPath"
    # Get .csproj files in the folder
    $csprojFiles = Get-ChildItem -Path $fullPath -Filter "*.csproj"
    foreach ($csprojFile in $csprojFiles) {
        # Load the .csproj file as an XML document
        $xml = [xml](Get-Content -Path $csprojFile.FullName)
        # Check if it's Flyingdarts.Persistence.csproj
        if ($csprojFile.Name -eq "Flyingdarts.Persistence.csproj") {
            $packageReferenceShared = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Flyingdarts.Shared" }
            $packageReferenceShared.Version = $sharedVersion
            
            $xml.Save($csprojFile.FullName)
        }
        # Check if it's Flyingdarts.Backend.Shared.csproj
        if ($csprojFile.Name -eq "Flyingdarts.Backend.Shared.csproj") {
            $packageReferenceShared = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Flyingdarts.Shared" }
            $packageReferenceShared.Version = $sharedVersion

            $packageReferencePersistence = $xml.Project.ItemGroup.PackageReference | Where-Object { $_.Include -eq "Flyingdarts.Persistence" }
            $packageReferencePersistence.Version = $persistenceVersion

            $xml.Save($csprojFile.FullName)
        }
    }

}
