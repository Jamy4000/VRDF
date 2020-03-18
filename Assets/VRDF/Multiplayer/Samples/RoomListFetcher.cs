using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Multiplayer.Absolute
{
    /// <summary>
    /// Response for the Refresh button
    /// WARNING : Need to be enable and gameObject active when we connect to the Master, or the room list won't be updated
    /// </summary>
    public class RoomListFetcher : MonoBehaviourPunCallbacks
    {
        [Header("Object containing the room list")]
        [SerializeField] private Transform _scrollviewContent;

        [Header("Button Allowing us to join a room")]
        [SerializeField] private GameObject _roomButton;

        private Dictionary<string, RoomInfo> _availableRooms = new Dictionary<string, RoomInfo>();

        /// <summary>
        /// Callback for when we receive an update on the list of available rooms
        /// WARNING : This callback does not provide a list of all the available room, only the one that changed and have been updated
        /// </summary>
        /// <param name="roomList">The list of rooms and there info</param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            UpdateCachedRoomList(roomList);
            DisplayRoomList();
        }

        private void DisplayRoomList()
        {
            foreach (Transform child in _scrollviewContent)
                Destroy(child.gameObject);

            foreach (var room in _availableRooms)
            {
                var newRoomButton = Instantiate(_roomButton, _scrollviewContent);
                newRoomButton.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = "Room: " + room.Key;
                newRoomButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => JoinRoom(room.Key));
            }

            void JoinRoom(string roomName)
            {
                PhotonNetwork.JoinRoom(roomName);
            }
        }

        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo info in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (info.RemovedFromList)
                {
                    if (_availableRooms.ContainsKey(info.Name))
                        _availableRooms.Remove(info.Name);

                    continue;
                }

                // Update cached room info
                if (_availableRooms.ContainsKey(info.Name))
                {
                    _availableRooms[info.Name] = info;
                }
                // Add new room info to cache
                else
                {
                    _availableRooms.Add(info.Name, info);
                }
            }
        }
    }
}