using Photon.Realtime;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Extension of the PUN Player class, add some VRSF Info to it
    /// </summary>
    public class VRSFPlayer : Player
    {
        /// <summary>
        /// The VR device used by this player
        /// </summary>
        public EDevice DeviceUsed = EDevice.NONE;

        /// <summary>
        /// The reference to the GameObject loaded at runtime by PUN from the Resources Folder
        /// </summary>
        public GameObject PlayerGameObject;

        public VRSFPlayer(Player basePlayer, GameObject playerInstance) : base(basePlayer.NickName, basePlayer.ActorNumber, basePlayer.IsLocal, basePlayer.CustomProperties)
        {
            PlayerGameObject = playerInstance;
        }
    }
}
