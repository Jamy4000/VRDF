using UnityEngine;
using System.Collections.Generic;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// A couple of static, utilities variable and Method to fetch the players in the scene
    /// </summary>
    public static class VRDFPlayerUtilities
    {
        /// <summary>
        /// Reference to the Local VRDF Player
        /// </summary>
        public static VRDFPlayer LocalVRDFPlayer;

        /// <summary>
        /// The list of all VRDF Players. Like PhotonNetwork.PlayersList, but for VRDF.
        /// </summary>
        public static List<VRDFPlayer> PlayersInstances = new List<VRDFPlayer>();

        /// <summary>
        /// If the player requested to go back to the connection room, or if it happened after a time out
        /// </summary>
        public static bool LocalPlayerHasRequestedToLeave = false;

        /// <summary>
        /// Find a Player in the PlayersInstances list
        /// </summary>
        /// <param name="playerID">The id of the player to look for</param>
        /// <returns>The player if it was found</returns>
        public static bool FindVRDFPlayer(string playerID, out VRDFPlayer VRDFPlayer, bool showErrorLog = true)
        {
            foreach (var player in PlayersInstances)
            {
                if (string.Equals(player.PhotonPlayer.UserId, playerID))
                {
                    VRDFPlayer = player;
                    return true;
                }
            }

            if (showErrorLog)
                Debug.LogErrorFormat("<b>[VRDF] :</b> Couldn't find player with userID {0}.", playerID);

            VRDFPlayer = null;
            return false;
        }
    }
}