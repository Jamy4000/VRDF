using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Keep track of the local player and of all other players in the room
    /// </summary>
    public class VRSFBasicPlayersManager : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// The local player instance. Use this to know if the local player is represented in the Scene
        /// </summary>
        public static GameObject LocalPlayerInstance;

        /// <summary>
        /// The local player instance. Use this to know if the local player is represented in the Scene
        /// </summary>
        public static List<VRSFPlayer> PlayersInstances = new List<VRSFPlayer>();

        // Use this for initialization
        protected virtual void Awake()
        {
            PlayersInstances.Clear();

            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Start()
        {
            PlayersInstances.Add(new VRSFPlayer(photonView.Owner, gameObject));
        }

        /// <summary>
        /// Called when a Photon Player got disconnected. We need to load a smaller scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerLeftRoom(Player other)
        {
            RemovePlayer(other, false);
        }

        /// <summary>
        /// Find a Player in the PlayersInstances list
        /// </summary>
        /// <param name="other">The player to look for</param>
        /// <returns>The player if it was found</returns>
        public static bool FindPlayer(Player other, out VRSFPlayer vrsfPlayer, bool showErrorLog = true)
        {
            foreach (var player in PlayersInstances)
            {
                if (player.NickName == other.NickName)
                {
                    vrsfPlayer = player;
                    return true;
                }
            }

            if (showErrorLog)
                Debug.LogErrorFormat("<b>[VRSF] :</b> Couldn't find player with userID {0}. Nickname of user should have been {1}.", other.UserId, other.NickName);

            vrsfPlayer = null;
            return false;
        }

        /// <summary>
        /// Remove a player from the PlayersInstances list whenever a player left the room
        /// </summary>
        /// <param name="toRemove">The player to remove</param>
        /// <param name="showCantFindPlayerError">Optional, if you want to hide the error whenever the player couldn't be removed, set this to false</param>
        public static void RemovePlayer(Player toRemove, bool showCantFindPlayerError = true)
        {
            foreach (var player in PlayersInstances)
            {
                if (player.UserId == toRemove.UserId)
                {
                    PlayersInstances.Remove(player);
                    return;
                }
            }

            if (showCantFindPlayerError)
                Debug.LogErrorFormat("<b>[VRSF] :</b> Couldn't find player with id {0} in VRSFPlayerManager.PlayersInstances. NickName was : {1}.", toRemove.UserId, toRemove.NickName);
        }
    }
}