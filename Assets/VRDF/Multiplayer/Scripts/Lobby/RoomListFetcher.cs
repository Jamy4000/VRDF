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
        /// <summary>
        /// Dictionary with all available rooms for this app, with the room name as kKey
        /// </summary>
        public static Dictionary<string, RoomInfo> AvailableRooms = new Dictionary<string, RoomInfo>();

        /// <summary>
        /// Callback for when we receive an update on the list of available rooms
        /// WARNING : This callback does not provide a list of all the available room, only the one that changed and have been updated
        /// </summary>
        /// <param name="roomList">The list of rooms and there info</param>
        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            base.OnRoomListUpdate(roomList);
            UpdateCachedRoomList(roomList);
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
                {
                    AvailableRooms.Add(room.Name, room);
                }
                // Update cached RoomInfo 
                else
                {
                    AvailableRooms[room.Name] =  room;
                }
            }
        }
    }
}