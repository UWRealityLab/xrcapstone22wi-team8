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
# https://github.com/flutter/flutter/blob/master/packages/flutter_tools/bin/macos_assemble.sh
# Note: this script is ran inside the ci_scripts folder
touch ../Flutter/ephemeral/FlutterInputs.xcfilelist
touch ../Flutter/ephemeral/FlutterOutputs.xcfilelist
