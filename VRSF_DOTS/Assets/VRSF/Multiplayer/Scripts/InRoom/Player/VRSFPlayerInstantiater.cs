using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Instantiate the player gameObject that was placed in the Resources/PhotonPrefabs folder
    /// </summary>
    public class VRSFPlayerInstantiater : MonoBehaviourPunCallbacks
    {
        [Header("The prefab representing the players")]
        [Tooltip("WARNING : Need to be placed in the Resources/PhotonPrefabs folder")]
        [SerializeField]
        private GameObject _playersPrefab;

        /// <summary>
        /// Keep track of the localPlayer position whenever a user leave/enter the room
        /// </summary>
        private static Vector3 _localPlayerPosCache = Vector3.zero;

        /// <summary>
        /// Keep track of the localPlayer rotation whenever a user leave/enter the room
        /// </summary>
        private static Quaternion _localPlayerRotCache = Quaternion.identity;

        public void Awake()
        {
            OnSetupVRReady.RegisterSetupVRResponse(LocalPlayerSetup);
        }

        public void OnDestroy()
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(LocalPlayerSetup))
                OnSetupVRReady.Listeners -= LocalPlayerSetup;
        }

        /// <summary>
        /// Callback for when a user enter the room. Keep the reference to the localPlayer Position and Rotation
        /// and destroy the instance of our player (Don't why, but it's needed by Photon apparently)
        /// </summary>
        /// <param name="otherPlayer">The player that entered the room</param>
        public override void OnPlayerEnteredRoom(Player otherPlayer)
        {
            PrepareUserReload();
        }
        /// <summary>
        /// Callback for when a user left the room. Keep the reference to the localPlayer Position and Rotation
        /// and destroy the instance of our player (Don't why, but it's needed by Photon apparently)
        /// </summary>
        /// <param name="otherPlayer">The player that left the room</param>

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            PrepareUserReload();
        }

        /// <summary>
        /// Prepare the reload of the user, as we reload the scene when a user leave/join the room
        /// </summary>
        private void PrepareUserReload()
        {
            _localPlayerPosCache = VRSF_Components.CameraRig.transform.position;
            _localPlayerRotCache = VRSF_Components.CameraRig.transform.rotation;

            // No idea why, but Photon require us to reload the instance of our local player
            if (VRSFBasicPlayersManager.LocalPlayerInstance != null)
                PhotonNetwork.Destroy(VRSFBasicPlayersManager.LocalPlayerInstance);
        }

        /// <summary>
        /// Setup the LocalPlayer instance whenever setupVR has been setup again
        /// </summary>
        /// <param name="info"></param>
        private void LocalPlayerSetup(OnSetupVRReady info)
        {
            if (_playersPrefab == null)
            {
                Debug.LogError("<b><Color=Red>[VRSF] :</b></Color> Missing playerPrefab Reference, can't refresh local player.", gameObject);
            }
            else
            {
                Debug.LogFormat("<b>[VRSF] :</b> We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                RefreshLocalPlayer();
            }

            /// <summary>
            /// Setup the LocalPlayer instance Transform
            /// </summary>
            void RefreshLocalPlayer()
            {
                // Check if there's no more instance of the LocalPlayer
                if (VRSFBasicPlayersManager.LocalPlayerInstance != null)
                    PhotonNetwork.Destroy(VRSFBasicPlayersManager.LocalPlayerInstance);

                // Set the CameraRig pos and rot based on the previous check
                VRSF_Components.CameraRig.transform.position = _localPlayerPosCache;
                VRSF_Components.CameraRig.transform.rotation = _localPlayerRotCache;

                // Instantiate the loacalPlayer Instance
                VRSFBasicPlayersManager.LocalPlayerInstance = PhotonNetwork.Instantiate(PlayerPrefabName, _localPlayerPosCache, _localPlayerRotCache);

                // Add the follower scripts for the Camera, RightController and LeftController
                AddFollowerScript(VRSFBasicPlayersManager.LocalPlayerInstance, VRSF_Components.VRCamera.transform);
                TryGetObjectWithName("RightController", VRSF_Components.RightController.transform);
                TryGetObjectWithName("LeftController", VRSF_Components.LeftController.transform);

                /// <summary>
                /// Try to fetch an object under the LocalPlayerInstance using a name. If found, we add a FollowerScript to it.
                /// </summary>
                void TryGetObjectWithName(string objectName, Transform toFollow)
                {
                    var toLookFor = VRSFBasicPlayersManager.LocalPlayerInstance.transform.Find(objectName);
                    if (toLookFor == null)
                        Debug.LogErrorFormat("<b>[VRSF] :</b> Couldn't find object with name {0} under the Player prefab. Not adding any Follower Script.", objectName);
                    else
                        AddFollowerScript(toLookFor.gameObject, toFollow);
                }

                /// <summary>
                /// Add a VRSFTransformFollower to the transToFollow object, so it can follow the position and rotation of the toFollow object
                /// </summary>
                void AddFollowerScript(GameObject transToFollow, Transform toFollow)
                {
                    VRSFTransformFollower transFollower = transToFollow.AddComponent<VRSFTransformFollower>();
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