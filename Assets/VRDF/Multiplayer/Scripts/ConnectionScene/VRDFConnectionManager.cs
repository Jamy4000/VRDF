using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// used to connect the user to the Server
    /// </summary>
    public class VRDFConnectionManager : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// The name or index of the scene you want to load as a multiplayer scene. 
        /// </summary>
        [Tooltip("The name or index of the scene you want to load as a multiplayer scene.")]
        public string MultiplayerSceneName = "MultiplayerScene";

        /// <summary>
        /// Singleton creation
        /// </summary>
        public static VRDFConnectionManager Instance; 

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                VRDF_Components.DebugVRDFMessage("Another instance of VRDFConnectionManager exist. Be sure to only have one VRDFConnectionManager in your Scene.", true);
                Destroy(this);
                return;
            }

            Instance = this;

            PhotonNetwork.AutomaticallySyncScene = true;

            // Check for when the user just left a room and is coming back to the Connection Scene
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                // Connect to Photon Online Server
                VRDF_Components.DebugVRDFMessage("Connecting with Photon Server ...");
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Callback for when a room was correctly created.
        /// </summary>
        public override void OnCreatedRoom()
        {
            if (!TryLoadScene())
            {
                VRDF_Components.DebugVRDFMessage("Can't load the Multiplayer Scene. Check the name and index of your multiplayer scene, and be sure that this scene was added in the Build Settings. Stopping app.", true);
                Application.Quit();
            }
        }
        /// <summary>
        /// Event raised when we want to create or join a room
        /// </summary>
        /// <param name="roomName">The name of the room we wanna create / join. Can be Empty if joining random room.</param>
        /// <param name="roomOptions">Contains the parameters for the room we want to create. Let it at null if you want to join a room</param>
        /// <param name="roomNeedCreation">Is the user joining a room or do we need to create it ?</param>
        /// <param name="connectToRandomRoom">Do you want to connect to a random room ?</param>
        public static void ConnectOrCreateRoom(string roomName, RoomOptions roomOptions = null, bool roomNeedCreation = true, bool connectToRandomRoom = false)
        {
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                VRDF_Components.DebugVRDFMessage("You are NOT connected to the Photon Servers. Call one of the PhotonNetwork.Connect method before trying to create / join a room.", true);
                return;
            }

            if (roomNeedCreation)
            {
                if (!PhotonNetwork.InLobby || !RoomListFetcher.AvailableRooms.ContainsKey(roomName))
                {
                    VRDF_Components.DebugVRDFMessage("Trying to create a Room with name: {0}.", debugParams: roomName);
                    // #Critical we need at this point to create a Room.
                    PhotonNetwork.CreateRoom(roomName, roomOptions);
                }
                else
                {
                    VRDF_Components.DebugVRDFMessage("Room with name {0} already exists, can't create it. Joining instead.", debugParams: roomName);
                    JoinRoom(connectToRandomRoom, roomName);
                }
            }
            else
            {
                JoinRoom(connectToRandomRoom, roomName);
            }
        }

        /// <summary>
        /// Try to load a scene based on its name
        /// </summary>
        /// <returns>true if the scene was correctly loaded</returns>
        public bool TryLoadScene()
        {
            try
            {
                VRDF_Components.DebugVRDFMessage("Trying to load the scene with name '{0}'.", debugParams: MultiplayerSceneName);
                PhotonNetwork.LoadLevel(MultiplayerSceneName);
                return true;
            }
            catch
            {
                VRDF_Components.DebugVRDFMessage("Couldn't load scene with name '{0}'.", true, MultiplayerSceneName);
                return false;
            }
        }

        /// <summary>
        /// Method called to join a room, simply call PhotonNetwork.JoinRandomRoom or PhotonNetwork.JoinRoom.
        /// </summary>
        /// <param name="connectToRandomRoom">Do you want to join a random room ?</param>
        /// <param name="roomName">The name of the room we want to join. Can be let empty if joining random room.</param>
        public static void JoinRoom(bool connectToRandomRoom = true, string roomName = "")
        {
            if (connectToRandomRoom)
            {
                VRDF_Components.DebugVRDFMessage("Joining Random Room");
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                VRDF_Components.DebugVRDFMessage("Joining Room {0}", debugParams: roomName);
                PhotonNetwork.JoinRoom(roomName);
            }
        }
    }
}