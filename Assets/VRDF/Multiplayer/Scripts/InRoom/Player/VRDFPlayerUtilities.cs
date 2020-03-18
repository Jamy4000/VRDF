using UnityEngine;
using System.Collections.Generic;

namespace VRDF.Multiplayer
{
    public static class VRDFPlayerUtilities
    {
        /// <summary>
        /// Reference to the Local Absolute Player
        /// </summary>
        public static VRDFPlayer LocalVRDFPlayer;

        /// <summary>
        /// The local player instance. Use this to know if the local player is represented in the Scene
        /// </summary>
        public static List<VRDFPlayer> PlayersInstances = new List<VRDFPlayer>();

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