using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Used to connect the user to the Server and create a connection to a room
    /// </summary>
    public class VRDFConnectionManager : MonoBehaviour
    {
        [Header("Photon Parameters")]
        [Tooltip("Do you want to connect to photon servers using settings on awake ?")]
        [SerializeField] private bool _connectToServerOnAwake = true;
        [Tooltip("Simply to set the PhotonNetwork.AutomaticallySyncScene on Awake")]
        [SerializeField] private bool _automaticallySyncScene = true;

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
            PhotonNetwork.AutomaticallySyncScene = _automaticallySyncScene;

            // Check for when the user just left a room and is coming back to the Connection Scene
            if (!PhotonNetwork.IsConnectedAndReady && _connectToServerOnAwake)
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