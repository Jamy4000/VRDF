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
        #region Variables
        [Header("Critical Room Parameters")]
        /// <summary>
        /// SHould we synchronize scenes between players or do you handle it yourself ?
        /// </summary>
        [Tooltip("Should we synchronize scenes between players or do you handle it yourself ")]
        public bool ShouldSynchronizeScene = true;
        /// <summary>
        /// The Maximal amount of player that can enter a room
        /// </summary>
        [Tooltip("The Maximal amount of player that can enter a room")]
        public int MaxPlayerPerRoom = 10;

        /// <summary>
        /// The name or index of the scene you want to load as a multiplayer scene. 
        /// </summary>
        [Tooltip("The name or index of the scene you want to load as a multiplayer scene.")]
        [SerializeField]
        public string MultiplayerSceneName;
        #endregion Variables


        #region Monobehaviours_Methods
        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        protected virtual void Awake()
        {
            // #Critical, this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
            PhotonNetwork.AutomaticallySyncScene = ShouldSynchronizeScene;

            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings();

            OnConnectionToRoomRequested.Listeners += ConnectToRoom;
        }

        protected virtual void OnDestroy()
        {
            OnConnectionToRoomRequested.Listeners -= ConnectToRoom;
        }
        #endregion Monobehaviours_Methods


        #region EventCallbacks
        /// <summary>
        /// Callback for when the user want to connect to a room.
        /// </summary>
        protected virtual void ConnectToRoom(OnConnectionToRoomRequested info)
        {
            // we check if we are connected, and join if we are
            if (PhotonNetwork.IsConnectedAndReady)
            {
                if (info.NeedCreation)
                {
                    Debug.Log("<b>[VRSF] :</b> Creating Room " + info.RoomName);
                    // #Critical we need at this point to create a Room.
                    PhotonNetwork.CreateRoom(info.RoomName, info.Options);
                }
                else
                {
                    JoinRoom(info.RoomName);
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>[VRSF] :</b> You're not connected to the Photon network. Please check that one of the PhotonNetwork.Connect method was called.</Color>");
            }
        }
        #endregion EventCallbacks


        #region PunCallbacks
        /// <summary>
        /// Callback for when the user could connect to the server.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("<b>[VRSF] :</b> Connection with Master established, joining Lobby ...");
            // When the user is connected to the server, we make him load a basic lobby so he can get the rooms info.
            PhotonNetwork.JoinLobby(new TypedLobby("BaseVRLobby", LobbyType.Default));
        }

        /// <summary>
        /// Callback for when the user could connect to the server.
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("<b>[VRSF] :</b> Lobby was successfully joined !");
            Debug.LogFormat("<b>[VRSF]:</b> {0} players, including this instance of the game, are currently online in your app.", PhotonNetwork.CountOfPlayers);
        }

        /// <summary>
        /// Callback for when a room was correctly created.
        /// </summary>
        public override void OnCreatedRoom()
        {
            Debug.Log("<b>[VRSF] :</b> The room was successfully CREATED, loading multipalyer scene ...");

            if (!TryLoadScene())
            {
                Debug.LogError("<Color=Red><b>[VRSF] :</b> Can't load the Multiplayer's Scene. Check the name and index of your multiplayer scene, and be sure that this scene was added in the Build Settings. Stopping app.</Color>", gameObject);
                Application.Quit();
            }
            else
            {
                PhotonNetwork.LeaveLobby();
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
            PhotonNetwork.LeaveLobby();
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

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            Debug.LogFormat("<b>[VRSF]:</b> {0} players are currently online in your app.", PhotonNetwork.CountOfPlayers);
        }
        #endregion PunCallbacks


        #region Private_Methods
        /// <summary>
        /// Try to load a scene based on its name
        /// </summary>
        /// <returns>true if the scene was correctly loaded</returns>
        private bool TryLoadScene()
        {
            try
            {
                Debug.LogFormat("<b>[VRSF] :</b> Trying to load the scene with name '{0}'", MultiplayerSceneName);
                PhotonNetwork.LoadLevel(MultiplayerSceneName);
                return true;
            }
            catch
            {
                Debug.LogFormat("<Color=Red><b>[VRSF] :</b> Couldn't load scene with name '{0}'.</Color>", MultiplayerSceneName);
                return false;
            }
        }
        #endregion Private_Methods


        #region Public_Methods
        public virtual void JoinRoom(string roomName)
        {
            Debug.Log("<b>[VRSF] :</b> Joining Room " + roomName);
            PhotonNetwork.JoinRoom(roomName);
        }
        #endregion Public_Methods
    }
}