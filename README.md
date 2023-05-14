# XRL Browser Oculus Demo

This app is a proof of concept demo for the XRL browser, built using Unity and XR Interaction Toolkit with TMP UI elements. It allows users to navigate websites in a virtual reality environment using Oculus controllers. This readme contains a userguide and general project overview. For more in-depth technical documentation, see [this](./TECHNICAL.md) readme.

## Setup

To run this project locally, follow these steps:

1. Ensure you have Unity installed (version 2021.3.25f1 or later) and the Oculus Integration package installed in your Unity project.
2. Clone the project from the GitHub repository to your local machine.
3. Open the project in Unity.
4. Go to the `Scenes` folder and open the `Main` scene.
5. Select the `XRL Browser` element, and ensure the `Server Url` parameter in the inspector window is set correctly. This url should be the same as the url of your XRL Backend instance. This value defaults to http://localhost:5000/ which is the default port for the XRL backend.
6. Connect your Oculus headset to your computer and open the Oculus app on your computer.
7. Press the play button in Unity to build and run the app on your Oculus headset.

## Usage

When the app starts, you will be placed in a virtual environment with a web browser window in front of you. To navigate websites, use the Oculus controllers as follows:

- **Pointing:** Point the controller at an element on the screen to select it.
- **Scrolling:** Use the thumbsticks on the controllers to scroll up and down the page.
- **Changing sites:** Use the dropdown menu on the right side of the screen to switch to a different website. The dropdown can be selected and scrolled using the controller's point and trigger actions.


## Acknowledgments

This project was built using the following resources:

- Unity (https://unity.com/)
- XR Interaction Toolkit (https://docs.unity3d.com/Packages/com.unity.xr.interaction.toolkit@latest/)
- TMP UI Elements (https://docs.unity3d.com/Packages/com.unity.textmeshpro@latest/)

Created by [Aaron Campbell](https://github.com/aaron-r-campbell) as part of a BSc at [Aalto University](https://www.aalto.fi).