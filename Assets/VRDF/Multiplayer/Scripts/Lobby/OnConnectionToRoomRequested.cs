using Photon.Realtime;

namespace VRDF.Multiplayer
{
    /// <summary>
    ///  Event raised when we want to create or join a room
    /// </summary>
    public class OnConnectionToRoomRequested : EventCallbacks.Event<OnConnectionToRoomRequested>
    {
        /// <summary>
        /// The name of the room we want to create / join
        /// </summary>
        public readonly string RoomName;

        /// <summary>
        /// Contains the parameters for the room we want to create. Let it at null if you want to join a room
        /// </summary>
        public readonly RoomOptions Options;

        /// <summary>
        /// Is the user joining a room or do we need to create it ?
        /// Let this to true if not sure. If you display a list of rooms, you need to set this to false when the user is Joining.
        /// </summary>
        public readonly bool NeedCreation;

        /// <summary>
        /// Event raised when we want to create or join a room
        /// </summary>
        /// <param name="roomName">The name of the room we wanna create / join</param>
        /// <param name="roomOptions">Contains the parameters for the room we want to create. Let it at null if you want to join a room</param>
        /// <param name="roomNeedCreation">Contains the parameters for the room we want to create. Let it at null if you want to join a room</param>
        public OnConnectionToRoomRequested(string roomName, RoomOptions roomOptions = null, bool roomNeedCreation = true) : base(" Event raised when we want to create or join a room")
        {
            RoomName = roomName;
            Options = roomOptions == null ? new RoomOptions { MaxPlayers = (byte)5 } : roomOptions;
            NeedCreation = roomNeedCreation;

            FireEvent(this);
        }
    }
}