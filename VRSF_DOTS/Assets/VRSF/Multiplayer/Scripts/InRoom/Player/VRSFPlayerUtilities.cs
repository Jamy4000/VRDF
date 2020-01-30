using UnityEngine;
using System.Collections.Generic;

namespace VRSF.Multiplayer
{
    public static class VRSFPlayerUtilities
    {
        /// <summary>
        /// Reference to the Local Absolute Player
        /// </summary>
        public static VRSFPlayer LocalVRSFPlayer;

        /// <summary>
        /// The local player instance. Use this to know if the local player is represented in the Scene
        /// </summary>
        public static List<VRSFPlayer> PlayersInstances = new List<VRSFPlayer>();

        /// <summary>
        /// Find a Player in the PlayersInstances list
        /// </summary>
        /// <param name="playerID">The id of the player to look for</param>
        /// <returns>The player if it was found</returns>
        public static bool FindVRSFPlayer(string playerID, out VRSFPlayer vrsfPlayer, bool showErrorLog = true)
        {
            foreach (var player in PlayersInstances)
            {
                if (string.Equals(player.PhotonPlayer.UserId, playerID))
                {
                    vrsfPlayer = player;
                    return true;
                }
            }

            if (showErrorLog)
                Debug.LogErrorFormat("<b>[VRSF] :</b> Couldn't find player with userID {0}.", playerID);

            vrsfPlayer = null;
            return false;
        }
    }
}