using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Multiplayer.Samples
{
    /// <summary>
    /// Simple sample script to create a room, and handle the "Create Room" button by checking the Photon network callbacks
    /// </summary>
    public class ConnectToRoom : Photon.Pun.MonoBehaviourPunCallbacks
    {
        [Header("Button used to create a room")]
        [SerializeField] private UnityEngine.UI.Button _createRoomButton;

        /// <summary>
        /// the name of the room we want to create
        /// </summary>
        private string _roomName = "Room Name";

        /// <summary>
        /// Callback for the Create Room Button in the Sample Lobby Scene
        /// </summary>
        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(_roomName))
                Debug.LogErrorFormat("<Color=red><b>[VRDF Sample]</b> Room name is empty, please provide a room name.");
            else 
                new OnConnectionToRoomRequested(_roomName);
        }

        /// <summary>
        /// Set the name of the room we want to create, using inputField onValueChanged callback
        /// </summary>
        /// <param name="newName"></param>
        public void SetRoomName(string newName)
        {
            _roomName = newName;
            CheckConnectButton();
        }

        /// <summary>
        /// Called when new rooms are fetched. Wait one frame for the RoomListFetcher to update the AvailableRooms list.
        /// </summary>
        /// <param name="roomList">The list of rooms and there info</param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            if (!Photon.Pun.PhotonNetwork.InLobby)
            {
                _createRoomButton.interactable = false;
                return;
            }

            StartCoroutine(WaitForOneFrame());

            IEnumerator<WaitForEndOfFrame> WaitForOneFrame()
            {
                yield return new WaitForEndOfFrame();
                CheckConnectButton();
            }
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            CheckConnectButton();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            CheckConnectButton();
        }

        /// <summary>
        /// Check if all requirement to create a room are reached, and set the interactibility of the connect button accordingly
        /// </summary>
        private void CheckConnectButton()
        {
            if (!string.IsNullOrEmpty(_roomName) && RoomListFetcher.AvailableRooms.ContainsKey(_roomName))
            {
                Debug.LogFormat("<b>[VRDF Sample]</b> Room with name {0} already exist, disabling create room button.", _roomName);
                _createRoomButton.interactable = false;
            }
            else if (!Photon.Pun.PhotonNetwork.IsConnectedAndReady || !Photon.Pun.PhotonNetwork.InLobby)
            {
                Debug.LogFormat("<b>[VRDF Sample]</b> The user isn't connected or isn't in the Lobby. Disabling create room button.");
                _createRoomButton.interactable = false;
            }
            else
            {
                _createRoomButton.interactable = true;
            }
        }
    }
}