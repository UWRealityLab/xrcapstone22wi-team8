---
title: "Week 8 Snippet"
date: 2022-03-07T12:28:40Z
# The following are extra (optional) metadata for ananke
# tags: ["tag1", "tag2"]
# https://unsplash.com/photos/SYTO3xs06fU, Photo by Marvin Meyer on Unsplash
featured_image: "https://images.unsplash.com/photo-1519389950473-47ba0277781c"
author: "Apollo Zhu"
description: "The End Is Insight"
---

Yueqian improved the scene setup and added brush functionalities; Kyle added instructions and reorganized the options menu; and Apollo worked on file importation, multiplayer and general bug fixes, and the weekly snippet you are reading right now.

<!--more-->

## New Features/Functionalities

You can read more about the [Boardless Final Alpha 1 Pre-release](https://github.com/UWRealityLab/xrcapstone22wi-team8/releases/tag/final-alpha) and [download the APK](https://github.com/UWRealityLab/xrcapstone22wi-team8/releases/download/final-alpha/xrcapstone22wi-team8-final-f4c0e17.apk) to try it out. See the Changelog section for the list of new features and functionalities.

## Review Requests

For the exact changes, you can checkout the `before-final` branch ane review [Pull Request #44](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/44):

- New Design
   - Boardless/Assets/Scenes/SampleScene.unity
   - Boardless/Assets/Scripts/PanelControl.cs in [Pull Request #40](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/40)
- Multiplayer
   - Boardless/Assets/Scripts/Networking/NetworkPlayer.cs and NetworkPlayerSpawner.cs
   - Boardless/Assets/Scripts/PlayerNameInputField.cs and Lobby.cs
   - Boardless/Assets/Scripts/Networking/FirebaseImageTexture.cs
- [Additional Files Types through Pull Request #35](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/35)
   - Boardless/Assets/Scripts/FirebaseFilesDropdown.cs
   - Boardless/Assets/Scripts/FirebaseServices.cs
- OCR
   - Boardless/Assets/Scripts/RecognizeText.cs
- Whiteboard
   - Boardless/Assets/Scripts/Whiteboard.cs
   - Boardless/Assets/Scripts/WhiteboardMarker.cs
   - Boardless/Assets/Scripts/Eraser.cs
- Object Manipulation and [Instructions through Pull Request #38](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/38)
   - Boardless/Assets/Scripts/ChangeScale.cs
   - Boardless/Assets/Scripts/MoveWithController.cs and [Rotation through Pull Request #39](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/39)

## Blocking Issues

Everything is going great!

## Up Next

We'll continue to improve on the multiplayer and whiteboard writing experience,
as well as working on general bug fixes and performance improvements.
If time allows, we'll also add the voice chat functionality.
