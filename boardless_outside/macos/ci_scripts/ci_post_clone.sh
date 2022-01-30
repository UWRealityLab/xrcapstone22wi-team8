#!/bin/sh
# https://developer.apple.com/documentation/xcode/writing-custom-build-scripts
# https://docs.flutter.dev/deployment/cd

# Install Dependencies
brew install cocoapods
brew install --cask flutter

# Setup Flutter Strcture
flutter config --enable-macos-desktop
flutter pub get
pod install