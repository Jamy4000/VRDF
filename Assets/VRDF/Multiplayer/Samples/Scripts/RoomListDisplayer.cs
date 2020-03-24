using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Multiplayer.Samples
{
    /// <summary>
    /// Display a simple list of available rooms
    /// </summary>
    public class RoomListDisplayer : MonoBehaviourPunCallbacks
    {
        [Header("Object to activate on Join Lobby")]
        [SerializeField] private GameObject _roomListPanel;

        [Header("Button to join Lobby")]
        [SerializeField] private UnityEngine.UI.Button _joinLobbyButton;

        [Header("Object containing the room list")]
        [SerializeField] private Transform _scrollviewContent;

        [Header("Button Allowing us to join a room")]
        [SerializeField] private GameObject _roomButton;

        private void Awake()
        {
            OnVRDFRoomsListWasUpdated.Listeners += UpdateDisplayedRoomList;
        }

        private void OnDestroy()
        {
            OnVRDFRoomsListWasUpdated.Listeners -= UpdateDisplayedRoomList;
        }

        /// <summary>
        /// Called from the Join Lobby button if the 
        /// </summary>
        public void JoinLobby()
        {
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby(new TypedLobby("VRDF Lobby", LobbyType.Default));
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            _joinLobbyButton.interactable = true;
        }

        /// <summary>
        /// Callback for when we receive an update on the list of available rooms
        /// </summary>
        /// <param name="info">The event containing the list of available rooms.</param>
        private void UpdateDisplayedRoomList(OnVRDFRoomsListWasUpdated info)
        {
            // This version is using the available rooms dictionary passed as parameters of the event
            DisplayRoomList(info.AvailableRooms);
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            ChangeUIElementState(true);
            // This version is using the available rooms dictionary that's available from the RoomListFetcher script
            DisplayRoomList(RoomListFetcher.AvailableRooms);
        }

        private void DisplayRoomList(Dictionary<string, RoomInfo> availableRooms)
        {
            // Destroy all previous buttons
            foreach (Transform child in _scrollviewContent)
                Destroy(child.gameObject);

            // Create a new button for each new available buttons.
            foreach (var room in availableRooms)
            {
                var newRoomButton = Instantiate(_roomButton, _scrollviewContent);
                newRoomButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Room Name: " + room.Key;
                newRoomButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListenerExtend(() => JoinRoom(room.Key));
            }

            /// <summary>
            /// Room button callback, simply call the ConnectOrCreateRoom method from VRDFConnectionManager
            /// </summary>
            void JoinRoom(string roomName)
            {
                VRDFConnectionManager.ConnectOrCreateRoom(roomName, roomNeedCreation: false);
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            // Destroy all room buttons, as the user isn't connected anymore
            foreach (Transform child in _scrollviewContent)
                Destroy(child.gameObject);

            ChangeUIElementState(false);
            // Shouldn't be interactable as user isn't online
            _joinLobbyButton.interactable = false;
        }

        private void ChangeUIElementState(bool isInLobby)
        {
            _joinLobbyButton.gameObject.SetActive(!isInLobby);
            _joinLobbyButton.interactable = !isInLobby;
            _roomListPanel.SetActive(isInLobby);
        }
    }
}