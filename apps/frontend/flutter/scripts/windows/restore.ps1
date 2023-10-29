# Function to recursively run 'flutter pub get'
function Run-FlutterPubGet {
  Get-ChildItem -Directory | ForEach-Object {
    $dir = $_.FullName
    if (Test-Path (Join-Path $dir "pubspec.yaml")) {
      Write-Host "Running 'flutter pub get' in $dir"
      Set-Location $dir
      flutter pub get
    }
    if (Test-Path $dir) {
      Set-Location $dir
      Run-FlutterPubGet
    }
  }
}

# Function to recursively run 'dart run build_runner build'
function Run-DartBuildRunner {
  Get-ChildItem -Directory | ForEach-Object {
    $dir = $_.FullName
    if (Test-Path (Join-Path $dir "pubspec.yaml")) {
      Write-Host "Running 'dart run build_runner build' in $dir"
      Set-Location $dir
      dart run build_runner build
    }
    if (Test-Path $dir) {
      Set-Location $dir
      Run-DartBuildRunner
    }
  }
}

# Run 'flutter pub get' recursively
Run-FlutterPubGet

# Run 'dart run build_runner build' recursively
Run-DartBuildRunner
