using Photon.Realtime;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Event raised when we want to connect to a room
    /// </summary>
    public class OnConnectionToRoomRequested : EventCallbacks.Event<OnConnectionToRoomRequested>
    {
        /// <summary>
        /// The name of the room we wanna join
        /// </summary>
        public readonly string RoomName;

        /// <summary>
        /// Contains th eparameters for the room we want to join
        /// </summary>
        public readonly RoomOptions Options;

        /// <summary>
        /// Is the user joining a room or do we need to create it ?
        /// Let this to true if not sure. If you display a list of rooms, you need to set this to false when the user is Joining.
        /// </summary>
        public readonly bool NeedCreation;

        /// <summary>
        /// Event raised when we want to connect to a room
        /// </summary>
        /// <param name="roomName">The name of the room we wanna join</param>
        /// <param name="needCreation">Contains th eparameters for the room we want to join</param>
        /// <param name="roomOptions">Is the user joining a room or do we need to create it ?</param>
        public OnConnectionToRoomRequested(string roomName, bool needCreation = true, RoomOptions roomOptions = null) : base("Event raised when we want to connect to a room")
        {
            RoomName = roomName;
            NeedCreation = needCreation;
            Options = roomOptions ?? new RoomOptions { MaxPlayers = (byte)5 };
            FireEvent(this);
        }
    }
}