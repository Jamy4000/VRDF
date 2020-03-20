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
        [Header("Object containing the room list")]
        [SerializeField] private Transform _scrollviewContent;

        [Header("Button Allowing us to join a room")]
        [SerializeField] private GameObject _roomButton;

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
            DisplayRoomList();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            // Destroy all room buttons, as the user isn't connected anymore
            foreach (Transform child in _scrollviewContent)
                Destroy(child.gameObject);
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
                new OnConnectionToRoomRequested(roomName, roomNeedCreation: false);
            }
        }
    }
}