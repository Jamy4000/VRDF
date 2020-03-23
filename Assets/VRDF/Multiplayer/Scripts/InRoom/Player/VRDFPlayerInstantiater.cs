using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Instantiate the player gameObject that was placed in the VRDF/Resources/PhotonPrefabs folder
    /// </summary>
    public class VRDFPlayerInstantiater : MonoBehaviourPunCallbacks
    {
        [Header("The prefab representing the players")]
        [Tooltip("WARNING : Need to be placed in the VRDF/Resources/PhotonPrefabs folder")]
        [SerializeField]
        private GameObject _playersPrefab;

        public void Awake()
        {
            OnSetupVRReady.RegisterSetupVRCallback(LocalPlayerSetup);
        }

        public void OnDestroy()
        {
            OnSetupVRReady.UnregisterSetupVRCallback(LocalPlayerSetup);
        }

        /// <summary>
        /// Setup the LocalPlayer instance whenever setupVR has been setup again
        /// </summary>
        private void LocalPlayerSetup(OnSetupVRReady _)
        {
            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { VRDFPlayer.DEVICE_USED, VRDF_Components.DeviceLoaded }
            });

            if (_playersPrefab == null)
            {
                VRDF_Components.DebugVRDFMessage("Missing playerPrefab Reference in VRDFPlayerInstantiater, can't instantiate local player.", true);
            }
            else
            {
                VRDF_Components.DebugVRDFMessage("We are Instantiating LocalPlayer from {0}", debugParams: SceneManagerHelper.ActiveSceneName);
                VRDFPlayerManager.LocalPlayerGameObjectInstance = PhotonNetwork.Instantiate(PlayerPrefabName, Vector3.zero, Quaternion.identity);
            }
        }

        private string PlayerPrefabName
        {
            get
            {
                return System.IO.Path.Combine("PhotonPrefabs", _playersPrefab.name);
            }
        }
    }
}