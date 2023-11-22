# Get the current working directory
$currentDirectory = Get-Location

# Function to remove bin and obj folders
function Remove-BinObjFolders {
    param (
        [string] $folderPath
    )

    # Check if the folder is 'bin' or 'obj' or '.turbo'
    if ($folderPath -match "\\bin$|\\obj$|\\node_modules$|\\.dart_tool|\\build|\\.angular") {
        $confirmation = 'Y'  # Automatically answer Yes to the confirmation prompt
        Write-Host "Removing folder: $folderPath"
        Remove-Item -Path $folderPath -Force -Recurse
        $folderPath
    }
}

# Recursively find and remove bin and obj and .turbo folders starting from the working directory
$deletedFolders = Get-ChildItem -Path $currentDirectory -Directory -Recurse | ForEach-Object {
    $folderPath = $_.FullName
    Remove-BinObjFolders -folderPath $folderPath
} | Where-Object { $_ -ne $null }

Write-Host "All bin, obj, .turbo, node_modules, .angular and .dart_tool folders removed starting from the working directory."
Write-Host "Deleted folder paths:`n$deletedFolders"
