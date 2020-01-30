using Photon.Realtime;
using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Extension of the PUN Player class, add some VRSF Info to it
    /// </summary>
    [System.Serializable]
    public class VRSFPlayer
    {
        /// <summary>
        /// The corresponding Photon player for this VRSFPlayer
        /// </summary>
        public Player PhotonPlayer;

        /// <summary>
        /// The reference to the GameObject loaded at runtime by PUN from the Resources Folder
        /// </summary>
        public GameObject PlayerGameObject;

        /// <summary>
        /// The parent GameObject for the player's body 3D Model, instantiated using VRSFPlayerModel
        /// </summary>
        public GameObject PlayerBodyModel;

        public VRSFPlayer(Player basePlayer, GameObject playerInstance)
        {
            PhotonPlayer = basePlayer;
            PlayerGameObject = playerInstance;
        }

        /// <summary>
        /// The name to get the Device used by this player in the Photon Player CustomProperties
        /// </summary>
        public const string DEVICE_USED = "DeviceUsed";
    }
}
