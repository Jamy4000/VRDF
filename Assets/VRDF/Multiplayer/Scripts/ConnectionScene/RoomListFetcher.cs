using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Refresh the list of cached rooms, as Photon doesn't have any Room List variable.
    /// </summary>
    public class RoomListFetcher : MonoBehaviourPunCallbacks
    {
        [UnityEngine.Tooltip("The name for your Lobby. If your clients have different Lobby names, they won't be able to see each others' rooms.")]
        public string LobbyName = "VRDF Lobby";

        [UnityEngine.Tooltip("Should we join the Lobby as soon as we are connected to the Server ?\n" +
            "If false, you will need to join the lobby yourself to fetch the online rooms.")]
        public bool JoinLobbyOnConnected = true;

        /// <summary>
        /// Dictionary with all available rooms for this app, with the room name as kKey
        /// </summary>
        private static Dictionary<string, RoomInfo> _availableRooms = new Dictionary<string, RoomInfo>();

        private void Awake()
        {
            _availableRooms.Clear();
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            // When the user is connected to the server, we make him join a basic lobby so he can get the list of rooms and their info.
            if (JoinLobbyOnConnected)
                PhotonNetwork.JoinLobby(new TypedLobby(LobbyName, LobbyType.Default));
        }

        /// <summary>
        /// Callback for when we receive an update on the list of available rooms
        /// WARNING : This callback does not provide a list of all the available room, only the one that changed and have been updated
        /// </summary>
        /// <param name="roomList">The list of rooms and there info</param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            UpdateCachedRoomList(roomList);
            new OnVRDFRoomsListWasUpdated(_availableRooms);
        }

        /// <summary>
        /// Update the Dictionary of Available rooms 
        /// </summary>
        /// <param name="roomList"></param>
        private void UpdateCachedRoomList(List<RoomInfo> roomList)
        {
            foreach (RoomInfo room in roomList)
            {
                // Remove room from cached room list if it got closed, became invisible or was marked as removed
                if (room.RemovedFromList)
                {
                    if (AvailableRooms.ContainsKey(room.Name))
                        AvailableRooms.Remove(room.Name);

                    continue;
                }

                // Add new room info to cache
                if (!AvailableRooms.ContainsKey(room.Name))
                    AvailableRooms.Add(room.Name, room);
                // Update cached RoomInfo 
                else
                    AvailableRooms[room.Name] =  room;
            }
        }

        public static Dictionary<string, RoomInfo> AvailableRooms
        {
            get
            {
                if (!PhotonNetwork.InLobby)
                    VRDF_Components.DebugVRDFMessage("You need to be in a Lobby to be able to access the room List ! Returning Empty Dictionary.", true);

                return _availableRooms;
            }

            private set => _availableRooms = value;
        }
    }
}