using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Instantiate the provided player as prefab gameObject.
    /// As Photon Requirement, this prefab needs to be placed in the Resources Folder
    /// </summary>
    public class VRDFPlayerInstantiater : MonoBehaviourPunCallbacks
    {
        [Header("Photon Player Prefab and Folder (NEEDS TO BE IN RESOURCES FOLDER)")]
        [Tooltip("Needs to be placed in the Resources folder")]
        public GameObject PlayersPrefab;

        [Tooltip("OPTIONAL: Folder in the Resources where the Player Prefab is placed.")]
        public string PlayersPrefabResourcesFolder = "PhotonPrefabs";

        protected virtual void Awake()
        {
            VRDFPlayerUtilities.PlayersInstances.Clear();

            if (PhotonNetwork.IsConnectedAndReady)
            {
                InstantiateLocalPlayer();
                OnSetupVRReady.RegisterSetupVRCallback(LocalPlayerSetup);
            }
        }

        protected virtual void OnDestroy()
        {
            OnSetupVRReady.UnregisterSetupVRCallback(LocalPlayerSetup);
        }

        /// <summary>
        /// Instantiate the local player using PhotonNetwork.Instantiate.
        /// Only needs to be done localy, as Photon automatically instantiate a Player GameObject for the remote players.
        /// </summary>
        protected void InstantiateLocalPlayer()
        {
            if (PlayersPrefab == null)
                VRDF_Components.DebugVRDFMessage("Missing playerPrefab Reference in VRDFPlayerInstantiater, can't instantiate local player.", true);
            else
                VRDFPlayerManager.LocalPlayerGameObjectInstance = PhotonNetwork.Instantiate(_playerPrefabName, Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Set the local player's device in its custom properties.
        /// </summary>
        protected void LocalPlayerSetup(OnSetupVRReady _)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { VRDFPlayer.DEVICE_USED, VRDF_Components.DeviceLoaded }
            });

            Destroy(this);
        }

        /// <summary>
        /// Provide the path in the resources folder for the player's prefab
        /// </summary>
        private string _playerPrefabName
        {
            get
            {
                return string.IsNullOrEmpty(PlayersPrefabResourcesFolder) ? PlayersPrefab.name : System.IO.Path.Combine(PlayersPrefabResourcesFolder, PlayersPrefab.name);
            }
        }
    }
}