using Photon.Realtime;
using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Contains all parameters for the Multiplayer part of VRSF
    /// </summary>
    //[CreateAssetMenu]
    public class VRMultiplayerParameters : Core.Utils.ScriptableSingleton<VRMultiplayerParameters>
    {
        [Header("Room Parameters")]
        /// <summary>
        /// The Maximal amount of player that can enter a room
        /// </summary>
        [Tooltip("The Maximal amount of player that can enter a room")]
        public int MaxPlayerPerRoom = 5;

        /// <summary>
        /// The Minimal amount of player that can enter a room
        /// </summary>
        [Tooltip("The Minimal amount of player that can enter a room")]
        public int MinPlayerPerRoom = 0;


        [Header("Current Room Reference")]
        /// <summary>
        /// The current room the user want to join/ has joined
        /// </summary>
        [Tooltip("The current room the user want to join/ has joined")]
        public RoomInfo CurrentRoom;
    }
}