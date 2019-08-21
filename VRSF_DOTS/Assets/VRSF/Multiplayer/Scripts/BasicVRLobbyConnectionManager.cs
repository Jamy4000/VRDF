using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// used to connect the user to the Lobby and display available rooms afterwards
    /// </summary>
    public class BasicVRLobbyConnectionManager : MonoBehaviourPunCallbacks
    {
        [Header("Rooms Parameters")]
        [Tooltip("The name or index of the scene you want to load as a multiplayer scene. One is enough.")]
        [SerializeField]
        private string _multiplayerSceneName;

        [Tooltip("The name or index of the scene you want to load as a multiplayer scene. One is enough.")]
        [SerializeField]
        [Range(0, 100)]
        private int _multiplayerSceneIndex;

        private List<RoomInfo> _onlineRooms = new List<RoomInfo>();

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        void Awake()
        {
            // #Critical, this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = true;

            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();

            OnConnectionToRoomRequested.Listeners += ConnectToRoom;
        }

        private void OnDestroy()
        {
            OnConnectionToRoomRequested.Listeners -= ConnectToRoom;
        }

        /// <summary>
        /// Callback for when the user could connect to the server.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            // When the user is connected to the server, we make him load a basic lobby so he can get the rooms info.
            PhotonNetwork.JoinLobby(new TypedLobby("BaseVRLobby", LobbyType.Default));
        }

        /// <summary>
        /// Callback for when the user could connect to a room.
        /// </summary>
        private void ConnectToRoom(OnConnectionToRoomRequested info)
        {
            // we check if we are connected, and join if we are
            if (PhotonNetwork.IsConnected)
            {
                if (info.NeedCreation)
                {
                    // if the room doesn't exist yet
                    if (ReallyNeedsCreation())
                    {
                        Debug.Log("<b>[VRSF] :</b> Creating Room " + info.RoomName);
                        // #Critical we need at this point to create a Room.
                        PhotonNetwork.CreateRoom(info.RoomName, info.Options);
                    }
                    else
                    {
                        JoinRoom();
                    }
                }
                else
                {
                    JoinRoom();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>[VRSF] :</b> You're not connected to the Photon network. Please check that one of the PhotonNetwork.Connect method was called.</Color>");
            }

            void JoinRoom()
            {
                Debug.Log("<b>[VRSF] :</b> Joining Room " + info.RoomName);
                // #Critical we need at this point to attempt joining a Room.
                PhotonNetwork.JoinRoom(info.RoomName);
            }

            bool ReallyNeedsCreation()
            {
                // We check if the room doesn't exist yet
                bool reallyNeedCreation = true;
                foreach (var room in _onlineRooms)
                {
                    if (room.Name == info.RoomName)
                        reallyNeedCreation = false;
                }
                return reallyNeedCreation;
            }
        }

        /// <summary>
        /// Callback for when a room was correctly created.
        /// </summary>
        public override void OnCreatedRoom()
        {
            Debug.Log("<b>[VRSF] :</b> The room was successfully CREATED, loading multipalyer scene ...");

            if (!TryLoadScene())
            {
                if (!TryLoadScene(_multiplayerSceneIndex))
                {
                    Debug.LogError("<Color=Red><b>[VRSF] :</b> Can't load the Multiplayer's Scene. Check the name and index of your multiplayer scene, and be sure that this scene was added in the Build Settings. Stopping app.</Color>", gameObject);
                    Application.Quit();
                }
            }
        }

        /// <summary>
        /// Callback for when we couldn't create a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("<Color=Red><b>[VRSF] :</b> The room couldn't be CREATED. Here's the return code :</Color>\n{0}.<Color=Red>\nAnd here's the message :</Color>\n{1}.", returnCode, message);
        }

        /// <summary>
        /// Callback for when a room was correctly joined.
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log("<Color=Green><b>[VRSF] :</b> The room was successfully JOINED, loading the scene ...</Color>");
        }

        /// <summary>
        /// Callback for when we couldn't join a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("<Color=Red><b>[VRSF] :</b> The room couldn't be JOINED. Here's the return code :\n{0}.\nAnd here's the message :\n{1}.</Color>", returnCode, message);
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            _onlineRooms = roomList;
        }

        /// <summary>
        /// Try to load a scene based on its build index or its name
        /// </summary>
        /// <param name="index">The index of the scene we wanna load</param>
        /// <returns>true if the scene was correctly loaded</returns>
        private bool TryLoadScene(int index = -1)
        {
            try
            {
                // Check if we provided a correct index for the room to load
                if (index != -1 && UnityEngine.SceneManagement.SceneManager.sceneCount > _multiplayerSceneIndex)
                {
                    Debug.LogFormat("<b>[VRSF] :</b> Trying to load the scene with index '{0}'", _multiplayerSceneIndex);
                    PhotonNetwork.LoadLevel(_multiplayerSceneIndex);
                    return true;
                }
                // If not, we try to load the scene using the _multiplayerSceneName variable of this script
                else
                {
                    Debug.LogFormat("<b>[VRSF] :</b> Trying to load the scene with name '{0}'", _multiplayerSceneName);
                    PhotonNetwork.LoadLevel(_multiplayerSceneName);
                    return true;
                }
            }
            catch
            {
                if (index != -1)
                    Debug.LogFormat("<Color=Red><b>[VRSF] :</b> Couldn't load scene with index '{0}'.</Color>", _multiplayerSceneIndex);
                else
                    Debug.LogFormat("<Color=Red><b>[VRSF] :</b> Couldn't load scene with name '{0}'.</Color>", _multiplayerSceneName);

                return false;
            }
        }
    }
}