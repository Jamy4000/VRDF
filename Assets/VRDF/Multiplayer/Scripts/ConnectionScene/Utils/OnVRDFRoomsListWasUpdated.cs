using System.Collections.Generic;
using Photon.Realtime;

/// <summary>
/// Event raised when the OnRoomListUpdated callback from Photon as been processed, and the room list in RoomListFetcher has been updated accordingly.
/// </summary>
public class OnVRDFRoomsListWasUpdated : EventCallbacks.Event<OnVRDFRoomsListWasUpdated>
{
    /// <summary>
    /// The list of currently available rooms. Can as well be accessed at any moment using the static field VRDF.Multiplayer.RoomListFetcher.AvailableRooms.
    /// </summary>
    public readonly Dictionary<string, RoomInfo> AvailableRooms;

    /// <summary>
    /// Event raised when the OnRoomListUpdated callback from Photon as been processed, and the room list in RoomListFetcher has been updated accordingly.
    /// </summary>
    /// <param name="availableRooms">The list of currently available rooms. Can as well be accessed at any moment using the static field VRDF.Multiplayer.RoomListFetcher.AvailableRooms.</param>
    public OnVRDFRoomsListWasUpdated(Dictionary<string, RoomInfo> availableRooms) : base("Event raised when the OnRoomListUpdated callback from Photon as been processed, and the room list in RoomListFetcher has been updated accordingly.")
    {
        AvailableRooms = availableRooms;
        FireEvent(this);
    }
}