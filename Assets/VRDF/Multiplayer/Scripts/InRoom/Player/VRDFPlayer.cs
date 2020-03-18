using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Extension of the PUN Player class, add some VRDF Info to it
    /// </summary>
    [System.Serializable]
    public class VRDFPlayer
    {
        /// <summary>
        /// The corresponding Photon player for this VRDFPlayer
        /// </summary>
        public Player PhotonPlayer;

        /// <summary>
        /// The reference to the GameObject loaded at runtime by PUN from the Resources Folder
        /// </summary>
        public GameObject PlayerGameObject;

        /// <summary>
        /// The parent GameObject for the player's body 3D Model, instantiated using VRDFPlayerModel
        /// </summary>
        public GameObject PlayerBodyModel;

        public VRDFPlayer(Player basePlayer, GameObject playerInstance)
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
