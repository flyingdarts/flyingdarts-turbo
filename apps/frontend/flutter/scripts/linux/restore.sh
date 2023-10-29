#!/bin/bash

# Function to recursively run 'flutter pub get'
function run_flutter_pub_get {
  for dir in */; do
    if [ -f "$dir/pubspec.yaml" ]; then
      echo "Running 'flutter pub get' in $dir"
      (cd "$dir" && flutter pub get)
    fi
    if [ -d "$dir" ]; then
      (cd "$dir" && run_flutter_pub_get)
    fi
  done
}

# Function to recursively run 'dart run build_runner build'
function run_dart_build_runner {
  for dir in */; do
    if [ -f "$dir/pubspec.yaml" ]; then
      echo "Running 'dart run build_runner build' in $dir"
      (cd "$dir" && dart run build_runner build)
    fi
    if [ -d "$dir" ]; then
      (cd "$dir" && run_dart_build_runner)
    fi
  done
}

# Run 'flutter pub get' recursively
run_flutter_pub_get

# Run 'dart run build_runner build' recursively
run_dart_build_runner
