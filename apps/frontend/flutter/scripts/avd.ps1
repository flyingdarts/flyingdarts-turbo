# Function to display a menu and get user input
function Show-Menu {
    param (
        [string]$Title = 'Choose an AVD',
        [string[]]$Options
    )

    Clear-Host
    Write-Host "$Title`n"

    for ($i = 0; $i -lt $Options.Length; $i++) {
        Write-Host ("{0}. {1}" -f ($i + 1), $Options[$i])
    }

    $selection = Read-Host "Enter the number of the AVD to start (1-$($Options.Length))"

    return $selection
}

function Start-AndroidEmulator($avdName) {
    $emulatorPath = 'C:\Users\mikep\AppData\Local\Android\Sdk\emulator\'
    $command = Join-Path $emulatorPath 'emulator.exe'
    $arguments = "-avd $avdName"
    
    $process = Start-Process -FilePath $command -ArgumentList $arguments -PassThru -NoNewWindow

    # Wait for the process to complete
    $process | Wait-Process
}

# Store the current directory
$currentDirectory = Get-Location

# Specify the emulator folder
$emulatorFolder = "C:\Users\mikep\AppData\Local\Android\Sdk\emulator\"

# Change to the emulator folder
Set-Location -Path $emulatorFolder

# Run the emulator command and store the output
$avds = .\emulator.exe -list-avds

# Change back to the original directory
Set-Location -Path $currentDirectory

# Convert the output to an array
$avdArray = $avds -split "`r`n" | Where-Object { $_ -ne '' }

# Print the AVD list using the Show-Menu function
$selectedAvdIndex = Show-Menu -Title "Choose an AVD" -Options $avdArray

# Get the selected AVD name
$selectedAvdName = $avdArray[$selectedAvdIndex - 1]

# Call the Start-AndroidEmulator function with the selected AVD name
Start-AndroidEmulator -avdName $selectedAvdName
