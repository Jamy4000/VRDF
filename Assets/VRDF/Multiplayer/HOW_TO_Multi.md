# VRSF Multiplayer Setup 

**This is a Step-by-Step tutorial on how to create your own VR multiplayer game using VRSF and Photon Engine.**
If you have any problem during the creation of your multiplayer app, do not hesitate to [send me an email](arnaudbriche1994@gmail.com) ! :)

## First part: Photon Setup 
1. You're going to need a Photon Engine connection to be able to use the multiplayer's features. To do so, go to https://www.photonengine.com/, Create an account / Login, create a new app of type "Photon PUN" and insert the name you want.

2. Copy the App ID of your app, you gonna need it soon.

3. Create a new Unity project, and import [PUN 2 from the Asset Store](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922) as well as [Photon Voice 2](https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518)  (**WARNING : Do not use the 1st version of PUN or Photon Voice, you absolutely need the 2nd version !**)

4. In Unity, go to Window > Photon Unity Network > PUN Wizard and click on Setup Project in the newly opened window.

5. Enter the App ID you copied in step 2, and press the "Setup Project" button.

6. The PhotonServerSettings should now be opened in your Inspector Window (if not, Window > Photon Unity Network > Highlight Server Settings).

7. Enter the same App ID in the "App ID Voice" field.

8. Setup a correct App Version number.


And that's it for Photon! You may need to redo the step 3 to 6 when you clone your project on another computer, but that's it for a basic Photon connection.



## Second part: VRDF Setup
1. [Follow the Basic Steps to setup VRSF in your project](https://github.com/Jamy4000/VRDF/wiki/Setup#basic-setup)

2. Import the VRDF_Multiplayer_vX.x.unitypackage from the [Release Page](https://github.com/Jamy4000/VRDF/releases) into your project


And everything is now ready for you to create your Multiplayer Environment! 
The next Steps below are here to help you understand what you exactly need to create a Lobby and a Multiplayer Scene with VRDF. You can skip those by simply copying the two scenes found in the Assets/VRDF/Multiplayer/Samples/Scenes folder.
**WARNING : Don't forget to add those scenes to the list of scenes in the Build Settings and to setup the following parameters:** 
- in the Lobby Scene, set the correct MultiplayerScene parameter in the BasicVRLobbyConnectionManager script,
- in the MultiplayerScene, set the correct LobbySceneName parameter in the VRDFMultiplayerGameManager script.



## Third Part: Multiplayer Scene Setup
The Multiplayer Scene will be the one in which all your player are gonna meet. You can of course create multiple, conencted scenes for that, but you shouldn't load them locally if you want to change from one to another (fo example, from the Waiting Room to the First Level). 

To do so, I encourage you to check the [PhotonNetwork Documentation](https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/gamemanager-levels/); it's a bit unclear sometimes, but basically you only want your Master Client to load the current Active Scene by calling PhotonNetwork.LoadLevel(NameOfScene), as Photon will take care of loading this scene for all your players (you can check the [PhotonNetwork.automaticallySyncScene parameter](https://doc-api.photonengine.com/en/pun/v2/class_photon_1_1_pun_1_1_photon_network.html#a4e0cda79eb8975010a09693e07afc7a6) as well for more info). 

So now, here's the Step-by-step guide on how to create a connected scene for your players to meet:
1. Create a new Scene (Duh).

2. _Remove the Main Camera GameObject from the Scene:_ It will be taken over by SetupVR later.

3. _Setup a basic environment:_ Just throw a plane and some cubes in there, it's just to have something to display to the user.

4. _Right click in the Hierarchy > VRDF > Add SetupVR to Scene:_ This will setup VR for your Local Player. For more info, [check the documentation](https://github.com/Jamy4000/VRDF/wiki/Core-Package#setupvr-and-friends). 

5. _Right click in the Hierarchy > VRDF > Multiplayer > In Room > Add Game Manager:_ This prefab contains two scripts:
* **VRDFMultiplayerGameManager:** Check if the player is correctly connected on start, and display some stuffs in the console for debug purposes.
* **VRDFPlayerInstantiater:** Instantiate the players (in DontDestroyOnLoad, Photon requirement) when they are entering the room.

For more info, [check the doc !](https://github.com/Jamy4000/VRDF/wiki/VRDF-Multiplayer-Extension)

6. Setup the parameters in the newly created Game Manager (Lobby Scene and Multiplayer Scene names)

7. Make sure that the scene you just created is in the Build Settings' scenes list

8. Optional : Add the MoveAround package to your project and add a flying mode or teleporter to let your players move in the environment.


**That's it for the Multiplayer Room !** Let's now create the lobby where the connection magic is happening.



## Fourth and Last Part: Lobby Setup
The lobby is where you check for the user connection with the Photon Service, where you create a new Room for your players to meet (warning: The room isn't the scene, a room is just an abstract place for the player to connect to. [Check the Photon Doc for more info !](https://doc-api.photonengine.com/en/pun/v1/class_room.html))

1. _Create a new Scene:_ Don't forget to add it to your Build Settings' Scene List

2. _Right click in the Hierarchy > VRDF > Multiplayer > Lobby > Connection Manager:_ This will create a new Connection Manager GameObject, which contains only one script: the [VRDFConnectionManager](https://github.com/Jamy4000/VRDF/blob/develop/Assets/VRDF/Multiplayer/Scripts/Lobby/VRDFConnectionManager.cs). This component handles the callbacks from Photon, and make sure that all parameters are present to establish a connection to a room. 

3. _Setup the VRDFConnectionManager parameters:_ Just fill the MultiplayerSceneName parameter, as well as the maximum amount of player in a room.

4. _Create a script to request a connection to a room:_ This script only needs one method, as follow:

```c#
public void CreateOrJoinRoom()
{
    // "Room Name" is the name of your room, it does NOT need to be the same name as your multiplayer scene. 
    // It can actually be whatever you, or the user, wants. 
    new VRDF.Multiplayer.OnConnectionToRoomRequested("Room Name");
}
```

This method will create a room with the name "Room name" if it doesn't exist yet, or join the room with this exact same name if it does exists. 

**WARNING: This event is only listened to if the current user is connected to a Lobby, meaning that the OnJoinedLobby Callback from Photon was raised !**  If you do use the VRSF Multiplayer extension, a message in your console will be displayed once you've entered the Lobby

5. _Call the above method:_ You've got the choice on this one: Create a "Connect" Button and link the OnClick response to the mehtod above, add it to the OnJoinedLobby callback from the MonoBehaviourPunCallbacks, ... Your call :).    

Aaaaand that's it ! Just enter playmode from your Lobby scene, and enjoy Multiplayer in VR !
