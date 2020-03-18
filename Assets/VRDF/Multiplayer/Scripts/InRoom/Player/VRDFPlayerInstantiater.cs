using Photon.Pun;
using UnityEngine;
using VRDF.Core.SetupVR;

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
        /// <param name="info"></param>
        private void LocalPlayerSetup(OnSetupVRReady info)
        {
            if (_playersPrefab == null)
            {
                Debug.LogError("<Color=Red><b>[VRDF] :</b></Color> Missing playerPrefab Reference, can't refresh local player.", gameObject);
            }
            else
            {
                Debug.LogFormat("<b>[VRDF] :</b> We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
                {
                    { VRDFPlayer.DEVICE_USED, VRDF_Components.DeviceLoaded }
                });

                InstantiateLocalPlayer();
            }

            /// <summary>
            /// Setup the LocalPlayer instance Transform
            /// </summary>
            void InstantiateLocalPlayer()
            {
                // Check if there's no more instance of the LocalPlayer
                if (VRDFPlayerManager.LocalPlayerGameObjectInstance != null)
                    PhotonNetwork.Destroy(VRDFPlayerManager.LocalPlayerGameObjectInstance);

                // Instantiate the loacalPlayer Instance
                VRDFPlayerManager.LocalPlayerGameObjectInstance = PhotonNetwork.Instantiate(PlayerPrefabName, Vector3.zero, Quaternion.identity);

                // Add the follower scripts for the Camera, RightController and LeftController
                AddFollowerScript(VRDFPlayerManager.LocalPlayerGameObjectInstance, VRDF_Components.VRCamera.transform);
                TryGetObjectWithName("RightController", VRDF_Components.RightController.transform);
                TryGetObjectWithName("LeftController", VRDF_Components.LeftController.transform);

                /// <summary>
                /// Try to fetch an object under the LocalPlayerInstance using a name. If found, we add a FollowerScript to it.
                /// </summary>
                void TryGetObjectWithName(string objectName, Transform toFollow)
                {
                    var toLookFor = VRDFPlayerManager.LocalPlayerGameObjectInstance.transform.Find(objectName);
                    if (toLookFor == null)
                        Debug.LogErrorFormat("<b>[VRDF] :</b> Couldn't find object with name {0} under the Player prefab. Not adding any Follower Script.", objectName);
                    else
                        AddFollowerScript(toLookFor.gameObject, toFollow);
                }

                /// <summary>
                /// Add a VRDFTransformFollower to the transToFollow object, so it can follow the position and rotation of the toFollow object
                /// </summary>
                void AddFollowerScript(GameObject transToFollow, Transform toFollow)
                {
                    VRDFTransformFollower transFollower = transToFollow.AddComponent<VRDFTransformFollower>();
                    transFollower.ToFollow = toFollow;
                }
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