#!/bin/sh
# https://developer.apple.com/documentation/xcode/writing-custom-build-scripts
# https://docs.flutter.dev/deployment/cd

# Install Dependencies
brew install cocoapods
brew install --cask flutter

# Setup Flutter Strcture
flutter config --enable-macos-desktop
cd boardless_outside
flutter pub get
if [ CI_PRODUCT_PLATFORM = 'macOS' ]
then
    flutter build macos --release
else
    flutter build ios --release
fi
