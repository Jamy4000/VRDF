using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// used to connect the user to the Lobby and display available rooms afterwards
    /// </summary>
    public class VRDFConnectionManager : MonoBehaviourPunCallbacks
    {
        [Header("Critical Room Parameters")]
        /// <summary>
        /// The name or index of the scene you want to load as a multiplayer scene. 
        /// </summary>
        [Tooltip("The name or index of the scene you want to load as a multiplayer scene.")]
        public string MultiplayerSceneName;

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        protected virtual void Awake()
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            Debug.Log("<b>[VRDF] :</b> Connecting with Master Server ...");
            PhotonNetwork.ConnectUsingSettings();
        }

        protected virtual void OnDestroy()
        {
            if (OnConnectionToRoomRequested.IsCallbackRegistered(ConnectOrCreateRoom))
                OnConnectionToRoomRequested.Listeners -= ConnectOrCreateRoom;
        }

        #region PunCallbacks
        /// <summary>
        /// Callback for when the user could connect to the server.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            Debug.Log("<Color=Green><b>[VRDF] :</b> Connection with Master established ! Trying to join Lobby ...</Color>");
            
            // When the user is connected to the server, we make him load a basic lobby so he can get the rooms info.
            PhotonNetwork.JoinLobby(new TypedLobby("BaseVRLobby", LobbyType.Default));

            // Add callback for when the user wants to connect to a room.
            OnConnectionToRoomRequested.Listeners += ConnectOrCreateRoom;
        }

        /// <summary>
        /// Callback for when the user could connect to the server.
        /// </summary>
        public override void OnJoinedLobby()
        {
            Debug.Log("<Color=Green><b>[VRDF] :</b> Lobby was successfully joined !</Color>");
            Debug.LogFormat("<b>[VRDF]:</b> {0} players, including this instance of the game, are currently online in your app.", PhotonNetwork.CountOfPlayers);
        }

        /// <summary>
        /// Callback for when a room was correctly created.
        /// </summary>
        public override void OnCreatedRoom()
        {
            Debug.Log("<Color=Green><b>[VRDF] :</b> The room was successfully <b>CREATED</b>, loading scene ...</Color>");

            if (!TryLoadScene())
            {
                Debug.LogError("<Color=Red><b>[VRDF] :</b> Can't load the Scene. Check the name and index of your multiplayer scene, and be sure that this scene was added in the Build Settings. Stopping app.</Color>", gameObject);
                Application.Quit();
            }
        }

        /// <summary>
        /// Callback for when we couldn't create a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("<Color=Red><b>[VRDF] :</b> The room couldn't be <b>CREATED</b>. Here's the return code :</Color>\n{0}.<Color=Red>\nAnd here's the message :</Color>\n{1}.", returnCode, message);
        }

        /// <summary>
        /// Callback for when a room was correctly joined.
        /// </summary>
        public override void OnJoinedRoom()
        {
            Debug.Log("<Color=Green><b>[VRDF] :</b> The room was successfully <b>JOINED</b>, loading the scene ...</Color>");
            // If we don't automatically sync the scenes between users, we load it locally
            if (!PhotonNetwork.AutomaticallySyncScene)
                UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(MultiplayerSceneName);
        }

        /// <summary>
        /// Callback for when we couldn't join a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("<Color=Red><b>[VRDF] :</b> The room couldn't be JOINED. Here's the return code :</Color> \n{0}.\nAnd here's the message :\n{1}.", returnCode, message);
        }

        /// <summary>
        /// Callback to display the amount of player in the room in the console
        /// </summary>
        /// <param name="lobbyStatistics"></param>
        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            Debug.LogFormat("<b>[VRDF]:</b> {0} players are currently online in your app.", PhotonNetwork.CountOfPlayers);
        }

        /// <summary>
        /// Callback for when the local player is disconnect. Simply remove the callback for OnConnectionToRoomRequested.
        /// </summary>
        /// <param name="cause"></param>
        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            // Remove callback for OnConnectionToRoomRequested if the user is disconnected
            if (OnConnectionToRoomRequested.IsCallbackRegistered(ConnectOrCreateRoom))
                OnConnectionToRoomRequested.Listeners -= ConnectOrCreateRoom;
        }
        #endregion PunCallbacks


        #region Other_Methods
        /// <summary>
        /// Callback for when the user want to connect to a room.
        /// </summary>
        protected virtual void ConnectOrCreateRoom(OnConnectionToRoomRequested info)
        {
            if (info.NeedCreation)
            {
                if (!RoomListFetcher.AvailableRooms.ContainsKey(info.RoomName))
                {
                    Debug.Log("<b>[VRDF] :</b> Creating Room with name: " + info.RoomName);
                    // #Critical we need at this point to create a Room.
                    PhotonNetwork.CreateRoom(info.RoomName, info.Options);
                }
                else
                {
                    Debug.LogFormat("<b>[VRDF] :</b> Room with name {0} already exist, can't create it. Joining instead.", info.RoomName);
                    JoinRoom(info.RoomName);
                }
            }
            else
            {
                JoinRoom(info.RoomName);
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
                Debug.LogFormat("<b>[VRDF] :</b> Trying to load the scene with name '{0}'", MultiplayerSceneName);

                // If we automatically sync the scenes with the MasterClient, we call the PhotonNetworkLoadLevel Method. If not, we need to load the scene locally.
                if (PhotonNetwork.AutomaticallySyncScene)
                    PhotonNetwork.LoadLevel(MultiplayerSceneName);
                else
                    UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(MultiplayerSceneName);

                return true;
            }
            catch
            {
                Debug.LogFormat("<Color=Red><b>[VRDF] :</b> Couldn't load scene with name '{0}'.</Color>", MultiplayerSceneName);
                return false;
            }
        }

        /// <summary>
        /// Method called to join a room, simply call PhotonNetwork.JoinRoom.
        /// </summary>
        /// <param name="roomName"></param>
        public void JoinRoom(string roomName)
        {
            Debug.Log("<b>[VRDF] :</b> Joining Room " + roomName);
            PhotonNetwork.JoinRoom(roomName);
        }
        #endregion Other_Methods
    }
}