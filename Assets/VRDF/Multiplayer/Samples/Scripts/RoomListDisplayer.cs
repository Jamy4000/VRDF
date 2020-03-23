using Photon.Pun;
using Photon.Realtime;
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

        /// <summary>
        /// Called from the Join Lobby button if the 
        /// </summary>
        public void JoinLobby()
        {
            if (!PhotonNetwork.InLobby)
                PhotonNetwork.JoinLobby();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnected();
            _joinLobbyButton.interactable = !PhotonNetwork.InLobby;
            _roomListPanel.SetActive(PhotonNetwork.InLobby);
        }

        /// <summary>
        /// Callback for when we receive an update on the list of available rooms
        /// WARNING : This callback does NOT provide a list of all the available room, only the one that changed and have been updated
        /// </summary>
        /// <param name="roomList">The list of rooms and there info</param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            // We wait for one frame to let the RoomListFetcher update the AvailableRooms Dictionary
            StartCoroutine(WaitForOneFrame());

            IEnumerator<WaitForEndOfFrame> WaitForOneFrame()
            {
                yield return new WaitForEndOfFrame();
                DisplayRoomList();
            }
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            _roomListPanel.SetActive(true);
            _joinLobbyButton.gameObject.SetActive(false);
            _joinLobbyButton.interactable = false;
            DisplayRoomList();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            // Destroy all room buttons, as the user isn't connected anymore
            foreach (Transform child in _scrollviewContent)
                Destroy(child.gameObject);

            _joinLobbyButton.gameObject.SetActive(true);
            _joinLobbyButton.interactable = false;
            _roomListPanel.SetActive(true);
        }

        private void DisplayRoomList()
        {
            foreach (Transform child in _scrollviewContent)
                Destroy(child.gameObject);

            foreach (var room in RoomListFetcher.AvailableRooms)
            {
                var newRoomButton = Instantiate(_roomButton, _scrollviewContent);
                newRoomButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Room Name: " + room.Key;
                newRoomButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => JoinRoom(room.Key));
            }

            void JoinRoom(string roomName)
            {
                VRDFConnectionManager.ConnectOrCreateRoom(roomName, roomNeedCreation: false);
            }
        }
    }
}