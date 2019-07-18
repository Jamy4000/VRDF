namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// 
    /// Represents the player's current use of the teleport machanic.
    /// </summary>
    public enum ETeleportState
    {
        /// The player is not using teleportation right now
        None,
        /// The player is currently selecting a teleport destination (holding down on touchpad)
        Selecting,
        /// The player has selected a teleport destination and is currently teleporting now (fading in/out)
        Teleporting
    }
}