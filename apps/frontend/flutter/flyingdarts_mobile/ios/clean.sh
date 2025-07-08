#!/bin/sh

rm -rf Podfile.lock
rm -rf Pods
# Uncomment line below if removing Pods and cleaning cache isn't enough.
# rm -rf ~/Library/Developer/Xcode/DerivedData
pod cache clean --all
pod repo update
pod install