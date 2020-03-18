# VR Framework using Unity DOTS
This repository is a Virtual Reality Framework using the Unity Input Manager for Inputs and Interaction, an Event System as seen in [Quill18 video](https://www.youtube.com/watch?v=04wXkgfd9V8) and the DOTS workflow from Unity3D. It aims to ease the use of Virtual Reality in a project, and to have a light tool for that, while integrating a cross-platform project, some basic VR features and being as fast as possible. 


There's still a couple of systems using MonoBehaviour during the PlayMode (Example : the LaserLengthSetter and the LaserWidthSetter), and by that I mean that they're not used to setup any Entity. 
Those cases are rare, but are most of the time used to link an Event, set on Awake, to a Unity Component that is still not implemented in ECS (Example : the LineRenderer for the LaserPointer).


# Documentation
For more info about this VR framework, you can check [the Wiki page](https://github.com/Jamy4000/VRDF/wiki). Everything should be described, from how to use all the framework's features to how they actually work in code.

I use the [Projects](https://github.com/Jamy4000/VRDF/projects) page and Issues for project management, new features and bug fixes. Don't hesitate to raise new issues / new cards in the project section ! 

For more info about the Event System we are using, please check the Github Repository and [video]((https://www.youtube.com/watch?v=04wXkgfd9V8)) provided above as well as [the example project I've created on my Github page](https://github.com/Jamy4000/UnityCallbackAndEventTutorial). This event system is really powerfull, and if it's not overused in a project (it can get messy quickly if you have too much events), it's perfect to communicate between Systems having to reference them.

If you have any in question on how to use this tool, you can either check the Wiki page, or I'll be glad to answer you, just send me an email to arnaudbriche1994@gmail.com ! :)

**WARNING**: I won't work on this repository for the next months as I'm gonna travel a lot, but I'll still try to keep the packages up-to-date, as well as my code according to the update from Unity.


# Releases
The stable versions are placed in [the Releases section](https://github.com/Jamy4000/VRDF/releases) of this repository. Multiple packages are available, with extensions depending on your use. The only one you absolutely need to use the framework is the VRDF_Core package.
