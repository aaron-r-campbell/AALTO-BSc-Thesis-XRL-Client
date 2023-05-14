# XRL Demo Client Technical Overivew

The XRL Browser Unity project contains a demo of the capabilies of the XRL Framework and browsing platform. The Unity project contains a browsing manager (`XRL Browser`), a UI Dropdown for selecting between availble demo sites (`Canvas`), and the necessary basic elements for an Oculus VR application (`XR Origin`, `XR Interaction Manager`, and `Directional Light`).

Most of the functionality of this demo app, beyond the basic boilerplate, is handled by 3 main scripts, `DropdownController`, `XRLManager`, and `TextureScroll`, which are discussed in the following sections.

## [DropdownController](./Assets/Scripts/DropdownController.cs)

This script is the first to load. It makes a call to `Server Url`/routes to get a json response containing routes for example and custom site urls from the `XRL Backend` server. It then populates the dropdown options with these sites, selects the first example site as the default option, and calls `XRLManager`'s `LoadPage()` funciton.

## [XRLManager](./Assets/Scripts/XRLManager.cs)

This script controls the loading and display of sites. The general flow of execution for this script is as follows:

1. `LoadPage(url)` is called (in the case of this application, by the DropdownController script, but this can be called by whatever as it is public) with a url parameter for the site to load.
2. The `LoadPage()` function querries the `XRL Backend` server's `/render?url=<url>` route with the url parameter receiving a json list of element screenshots (width, height, url) and thier location in the XRL layout from the server.
3. The `LoadPage()` function makes calls to the `CreatePlanesForImageList()` Script for each of the major frame-paths in the XRL layout; it uses the offset caluculated for the main path to control for branching path positions.
4. The `CreatePlanesForImageList()` script calculates for each image the appropriate width and height (based on a pixel -> mm conversion) and creates a plane for the image with the appropriate dimensions and offset. This function makes calls to `ApplyImageTexture()` which in turn calls `DownloadTextureAsync()` adding the correct texture to this image.

## [TextureScroll](./Assets/Scripts/TextureScroll.cs.cs)

This script is loaded as part of the `ApplyImageTexture()` function in the `XRLManager`'s execution. This script takes some info about the image, finds the XR Controllers in the scene, and creates an interaction for the XR Controllers to scroll the image texture using the thumbsticks. This emulates how the XRL Framework deals with container modals creating indipendantly scrollable frames.