---
title: "Week 5 Snippet"
date: 2022-02-11T07:00:00-08:00
# The following are extra (optional) metadata for ananke
# tags: ["tag1", "tag2"]
# https://giphy.com/gifs/GVMhZwYv8U5NK, Kevin Durant MVP GIF By NBA
featured_image: "https://media3.giphy.com/media/GVMhZwYv8U5NK/giphy.gif"
author: "Apollo Zhu"
description: "YOU DA REAL MVP"
---

We released our [MVP](https://github.com/UWRealityLab/xrcapstone22wi-team8/releases/tag/mvp-rc.2) on Tuesday and continued working towards the target goal according to plan.

<!--more-->

## New Features/Functionalities

- Use the dropdown menu on the dashboard to add plain text contents to the virtual workspace 
  <iframe width="589" height="339" src="https://www.youtube-nocookie.com/embed/Vcnx7S1Zh74?start=2" title="Boardless - File Transfer Client" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

  <iframe width="589" height="589" src="https://www.youtube-nocookie.com/embed/16W1N3Jn1Bk" title="Boardless - File Dropdown" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

  ![Transferred text content overlays behind a whiteboard with the word Yes written on it](../../images/week5/draw-on-file.png)
- Move and scale primitive 3D objects or plain text contents with the right and the left controller
  <iframe width="589" height="338" src="https://www.youtube-nocookie.com/embed/yyZRPcLZ9qE" title="Boardless - 2D Brush & 3D Shape Generation" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
- Multiple participants can join the same room and manipulate the 3D objects together
  <iframe width="589" height="589" src="https://www.youtube-nocookie.com/embed/1c-GHKjepkY" title="Boardless - Multi-user Object Move & Scale" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
- Dark mode, deletion, and loading animation support in file transfer client

## Review Requests

- [#17](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/17): Firebase setup in Unity
- [#18](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/18): 3D object move and scale
- [#21](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/21): file transfer client improvements
- [#26](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/26): mvp integration, which includes
  - combining object generation dashboard with drawing options dashboard
  - fixing 3D primitives floating away/having wrong rotation
  - [#20](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/20): transferred file selection + plain text display
  - [#25](https://github.com/UWRealityLab/xrcapstone22wi-team8/pull/25): basic multi-user support

## Blocking Issues

- We are unsure why our Whiteboard script doesn't work with anything other than a plane.
- <# TODO #>
