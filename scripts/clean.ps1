# Get the current working directory
$currentDirectory = Get-Location

# Function to remove bin and obj folders
function Remove-BinObjFolders {
    param(
        [string] $folderPath
    )

    # Check if the folder is 'bin' or 'obj'
    if ($folderPath -match "\\bin$|\\obj$") {
        Write-Host "Removing folder: $folderPath"
        Remove-Item -Path $folderPath -Force -Recurse
    }
}

# Recursively find and remove bin and obj folders only in 'apps' and 'packages' folders
Get-ChildItem -Path $currentDirectory -Directory -Recurse | ForEach-Object {
    $folderPath = $_.FullName
    $parentFolder = (Split-Path -Path $folderPath -Leaf)

    if ($parentFolder -eq 'apps' -or $parentFolder -eq 'packages') {
        Remove-BinObjFolders -folderPath $folderPath
    }
}

Write-Host "All bin and obj folders removed in 'apps' and 'packages' folders."
