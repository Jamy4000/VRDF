# VR Scriptable Framework using Unity DOTS
This repository is a Virtual Reality Framework using the Unity Input Manager for Inputs and Interaction, an Event System as seen in [Quill18 video](https://www.youtube.com/watch?v=04wXkgfd9V8) and the DOTS from Unity3D. It aims to ease the use of Virtual Reality in a project, and to have a light tool for that, while integrating a cross-platform project, some basic VR features and being as fast as possible. 


There's still a couple of systems using MonoBehaviour during the PlayMode (Example : the LaserLengthSetter and the LaserWidthSetter), and by that I mean that they're not used to setup any Entity. 
Those cases are rare, but are most of the time used to link an Event, set on Awake, to a Unity Component that is still not implemented in ECS (Example : the LineRenderer for the LaserPointer).
If you have any in question on how to use this tool, I'll be glad to answer you, just send me an email to arnaudbriche1994@gmail.com ! :)


## Description
The repository you're currently is a Crossfplatform, Lightweight VR Framework giving you access to some basic features often used in VR (UI, Teleportation, Flying mode, Gaze, Inputs Management, ...). It's an alternative to Libraries like VRTK, that was way too big for me when I first used it.


The supported devices for now are :
- The HTC Vive
- The HTC Focus (No 3D Models for the controllers are provided for now)
- The Microsoft Mixed Reality Headset
- The Oculus Rift with Touch Controllers
- The Oculus Quest
- The Oculus GO
- The Gear VR
- A VR Simulator (only recommended for debug)


# Releases
The stable versions are placed in the Releases section of this repository. Multiple packages are available, with extensions depending on your use. The only one you absolutely need is the VRSF_Hybrid_Core package.


# Requirements
For Unity, you need to download the latest **2019.1 version or later**, as it's required to be able to use Unity DOTS.

## Packages
To use this Framework, you gonna need the following stuffs :
- **The XR Legacy Input Helpers** : You can find it in the Package Manager from Unity (in Unity, Tab Window > Package Manager, in the Packages Window click on All Packages > XR Legacy Input Helpers > Install).
- **The Entities Package 0.0.12 preview-33** : You can find it in the Package Manager from Unity (in Unity, Tab Window > Package Manager, in the Packages Window click on Advanced > Show Preview Packages, and then : All Packages > Entities > Install). 
- **The Hybrid Renderer Package 0.0.1 preview-13** : You can find it in the Package Manager from Unity (in Unity, Tab Window > Package Manager, in the Packages Window click on Advanced > Show Preview Packages, and then : All Packages > Hybrid Renderer > Install). 

## Optional Packages
You still need to import some VR Packages, depending on your needs, to use this framework. Those are found in the Package Manager from Unity :
- **Oculus (Desktop)** : If you want to use the Rift or Rift S Support
- **Oculus (Android)** : If you want to use the Oculus Go, Gear VR or Oculus Quest Support
- **OpenVR (Desktop)** : If you want to use the HTC Vive or HTC Focus (**WARNING :** MODELS FOR FOCUS CONTROLLERS NOT PROVIDED)
- **Windows Mixed Reality** : If you want to use the WMR Headset

### Oculus GO, Oculus Quest, Gear VR and HTC Focus Specifities
If you need to build for a mobile platform, you need as well to download the Android Building support (File > Build Settings > Android) and to switch the platform to Android.

## Other Settings
- **VR Support** : In the Player Settings Window (Edit > Project Settings > Player), go to the last tab called XR Settings, set the Virtual Reality Supported toggle to true, and add the Oculus, OpenVR and None SDKs to the list.
- **Scripting Runtime Version** : This one is normally set by default in the last versions of Unity, but we never know :  still in the Player Settings Window, go to the Other Settings tab and set the Scripting Runtime version to .NET 4.x Equivalent.



Once all of that is done, **Restart your project so everything can be recompiled !**



# Basic Setup

1. Import the different packages and setup the settings listed above
2. Relaunch the Editor to be sure that everything is correctly recompiled
3. Import the VRSF_DOTS_Core package
4. Import the other VRSF_DOTS extension packages you need (Samples, Move Around, UI or Gaze)
5. Go to Edit > ProjectSettings > Input and use the Preset button on the top right corner to set the Inputs to the preset included in the Core Package from VRSF
6. Go to Edit > Player > Project Settings > XR Settings and tick the Virtual Reality Supported checkbox
7. Add, in this order, the Oculus SDK, OpenVR SDK, and None (For the Simulator)
8. Add SetupVR in your scene (Right click in Scene View > VRSF > Add SetupVR in Scene)
9. Set the Start Position of your CameraRig using the CameraRig object
10. You should be good to go !

If you want to add anything more in your scene (Movements, UI, Gaze, ...), just check the prefabs in the different Extension Packages, or check the different scenes in the VRSF.Samples folder of this repository :)


# Credits
This repository is based on multiple repositories found online, and that's why I would like to thanks the following persons for their work that helped me through the development of this project :
- The EventCallbacks Plugin from [Quill18](https://www.youtube.com/watch?v=04wXkgfd9V8) and the rewriting of it by [CrazyFrog55](https://github.com/crazyfox55) and [FuzzyHobo](https://github.com/FuzzyHobo). I made my own version available [here](https://github.com/Jamy4000/UnityCallbackAndEventTutorial).
- The Vive-Teleporter offered by [FlaFla2](https://github.com/Flafla2/Vive-Teleporter) for the calculation and display of the Parabole in the Curve Teleporter.


# Documentation
For more info about this VR framework, please send me a message, as the Wiki is still a work in progress.

For more info about the Event System we are using, please check the Github Repository and video given above as well as the example project I've created on my Github page.
