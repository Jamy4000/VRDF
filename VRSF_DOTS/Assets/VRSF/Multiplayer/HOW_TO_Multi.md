# VRSF Multiplayer Setup 

**This is a Step-by-Step tutorial on how to create your own VR multiplayer game using VRSF and Photon.**
If you have any problem during the creation of your multiplayer app, do not hesitate to send me an email ! :)

## First part : Photon Setup 
1. You're going to need a Photon connection to be able to use multiplayer. For that, go to https://www.photonengine.com/, Create an account / Login, Create a new app with type "Photon PUN" and the name you want.

2. Copy the App Id of your app, you gonna need it soon

3. Create a new Unity project, and import [PUN 2 from the Asset Store](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) as well as [Photon Voice 2](https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518)  (WARNING : Do not use the normal PUN or Photon Voice, you absolutely need the 2nd version !)

4. In Unity, go to Window > Photon Unity Network > PUN Wizard and click on Setup Project in the newly opened window.

5. Enter the app id you copied in the 2nd step, and then press Setup Project.

6. The PhotonServerSettings should now be opened in your inspector (if not, Window > Photon Unity Network > Highlight Server Settings).
Enter the same App Id in the App Id Voice field.

7. Setup a correct App Version number.


And you're done with Photon ! You may need to redo the step 3 to 6 when you clone your project on another computer, but that's it for a basic connection.



## Second part : VRSF Setup
1. [Follow the Basic Steps to setup VRSF in your project until the Step 9](https://github.com/Jamy4000/VRSF_DOTS#basic-setup)

2. Import the VRSF_DOTS_Multiplayer_vX.x.unitypackage in your project


For the next Steps, you can simply copy the two scenes found under VRSF/Multiplayer/Samples.
WARNING : Don't forget to add them to the list of scenes in the Build Settings and to setup the correct parameters 
- in the Connection Manager for the ConnectionScene
- in the Game Manager for the MultiRoom



## Third Part: Multiplayer Scene Setup
1. Create a new Scene : This will be the scene where all your players meet.

2. Remove the camera GameObject from the scene

3. Right click in the Hierarchy > VRSF > Multiplayer > Add SetupMultiVR to Scene

4. Right click in the Hierarchy > VRSF > Multiplayer > Add Game Manager

5. Setup the parameters in the newly created Game Manager (Lobby Scene and Multiplayer Scene names)

6. Create a small environment for your players

7. Make sure that the scene that you just created is in the list of Scenes of the Build Settings

8. Optional : Add the MoveAround package to your project and add a flying mode or teleporter to let your players move in the scene.
If you use the teleporters, DO NOT FORGET TO SETUP THE NAVMESH.


That's it for the Multiplayer Room, let's now create the lobby where the connection magic is happening.



## Fourth and Last Part: Lobby Setup
1. Create a new Scene : This will be your connection scene for Photon, or your Lobby.

2. Add a new object called Connection Manager and add the Basic VR Lobby Connection Manager script to it.

3. Setup the Multiplayer Scene Name and/or Multiplayer Scene Index. The two will be checked when loading the new Multiplayer scene.

4. Create a monobehaviour script that only need those line of code :

```c#
private void Awake()
{
    new OnConnectionToRoomRequested("My Room Name");
}
```

This will create a room with the name "My Room name" that can contain a maximum of 5 players.

5. Make sure that the scene that you just created is in the list of Scenes of the Build Settings


That's it for the lobby, you should now be good to go ! Just start the playmode from your Lobby scene, and enjoy Multiplayer in VR !