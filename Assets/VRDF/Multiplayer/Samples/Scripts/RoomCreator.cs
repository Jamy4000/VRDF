using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Multiplayer.Samples
{
    /// <summary>
    /// Simple sample script to create a room, and handle the "Create Room" button by checking the Photon network callbacks
    /// </summary>
    public class RoomCreator : Photon.Pun.MonoBehaviourPunCallbacks
    {
        [Header("Button used to create a room")]
        [SerializeField] private UnityEngine.UI.Button _createRoomButton;

        /// <summary>
        /// the name of the room we want to create
        /// </summary>
        private string _roomName = "Room Name";

        /// <summary>
        /// Callback for the Create Room Button in the Sample Connection Scene
        /// </summary>
        public void CreateRoom()
        {
            if (string.IsNullOrEmpty(_roomName))
                Debug.LogErrorFormat("<Color=red><b>[VRDF Sample]</b> Room name is empty, please provide a room name.");
            else
                VRDFConnectionManager.ConnectOrCreateRoom(_roomName, new RoomOptions { MaxPlayers = 5, PlayerTtl = 10000 });
        }

        /// <summary>
        /// Set the name of the room we want to create, using inputField onValueChanged callback
        /// </summary>
        /// <param name="newName"></param>
        public void SetRoomName(string newName)
        {
            _roomName = newName;
            CheckCreateRoomButton();
        }

        /// <summary>
        /// Called when new rooms are fetched. Wait one frame for the RoomListFetcher to update the AvailableRooms list.
        /// </summary>
        /// <param name="roomList">The list of rooms and there info</param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            StartCoroutine(WaitForOneFrame());

            IEnumerator<WaitForEndOfFrame> WaitForOneFrame()
            {
                yield return new WaitForEndOfFrame();
                CheckCreateRoomButton();
            }
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            CheckCreateRoomButton();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            CheckCreateRoomButton();
        }

        /// <summary>
        /// Check if all requirement to create a room are reached, and set the interactibility of the connect button accordingly
        /// </summary>
        private void CheckCreateRoomButton()
        {
            if (Photon.Pun.PhotonNetwork.InLobby && RoomListFetcher.AvailableRooms.ContainsKey(_roomName))
            {
                VRDF_Components.DebugVRDFMessage("Room with name {0} already exist, disabling create room button.", debugParams: _roomName);
                _createRoomButton.interactable = false;
            }
            else if (!Photon.Pun.PhotonNetwork.IsConnectedAndReady)
            {
                VRDF_Components.DebugVRDFMessage("The user isn't connected to Game Server. Disabling create room button.");
                _createRoomButton.interactable = false;
            }
            else if (string.IsNullOrEmpty(_roomName) || string.IsNullOrWhiteSpace(_roomName))
            {
                _createRoomButton.interactable = false;
            }
            else
            {
                _createRoomButton.interactable = true;
            }
        }
    }
}