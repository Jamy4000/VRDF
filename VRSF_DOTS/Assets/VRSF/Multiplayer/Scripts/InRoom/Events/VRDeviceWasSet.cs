namespace VRSF.Multiplayer
{
    /// <summary>
    /// Event raised when we received and set a VR Device to one of our player
    /// </summary>
    public class VRDeviceWasSet : EventCallbacks.Event<VRDeviceWasSet>
    {
        /// <summary>
        /// The player that has send its device info
        /// </summary>
        public readonly VRSFPlayer Player;

        /// <summary>
        /// Event raised when we received and set a VR Device to one of our player
        /// </summary>
        /// <param name="player">The player that has send its device info</param>
        public VRDeviceWasSet(VRSFPlayer player) : base("Event raised when we received and set a VR Device to one of our player")
        {
            Player = player;
            FireEvent(this);
        }
    }
}