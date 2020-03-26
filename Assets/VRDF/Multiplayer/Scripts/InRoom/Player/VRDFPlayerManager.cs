using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Keep track of the local player and of all other players in the room
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class VRDFPlayerManager : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        public VRDFPlayer ThisPlayer;

        /// <summary>
        /// The local player instance. 
        /// Use this to get the local player gameObject in DontDestroyOnLoad.
        /// </summary>
        public static GameObject LocalPlayerGameObjectInstance;

        protected virtual void Awake()
        {
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
            ThisPlayer = new VRDFPlayer(gameObject);
        }

        protected virtual void Start()
        {
            // Need to do that on Start, as photonView may not be setup on Awake.
            ThisPlayer.PhotonPlayer = photonView.Owner;

            if (photonView.IsMine)
            {
                VRDFPlayerUtilities.LocalVRDFPlayer = ThisPlayer;
                LocalPlayerGameObjectInstance = gameObject;
                AddFollowerScripts();
            }

            VRDFPlayerUtilities.PlayersInstances.Add(ThisPlayer);
        }

        protected virtual void OnDestroy()
        {
            VRDFPlayerUtilities.PlayersInstances.Remove(ThisPlayer);
        }

        /// <summary>
        /// Setup the LocalPlayer instance Transform
        /// </summary>
        protected void AddFollowerScripts()
        {
            // Add the follower scripts for the Camera, RightController and LeftController
            AddFollowerScriptToOneObject(LocalPlayerGameObjectInstance, VRDF_Components.VRCamera.transform);
            TryGetObjectWithName("RightController", VRDF_Components.RightController.transform);
            TryGetObjectWithName("LeftController", VRDF_Components.LeftController.transform);

            /// <summary>
            /// Try to fetch an object under the LocalPlayerInstance using a name. If found, we add a FollowerScript to it.
            /// </summary>
            void TryGetObjectWithName(string objectName, Transform toFollow)
            {
                var toLookFor = LocalPlayerGameObjectInstance.transform.Find(objectName);
                if (toLookFor == null)
                    VRDF_Components.DebugVRDFMessage("Couldn't find object with name {0} under the Local Player Instance. Not adding any Follower Script.", true, objectName);
                else
                    AddFollowerScriptToOneObject(toLookFor.gameObject, toFollow);
            }
        }

        /// <summary>
        /// Add a VRDFTransformFollower to the transToFollow object, so it can follow the position and rotation of the toFollow object
        /// </summary>
        protected void AddFollowerScriptToOneObject(GameObject transToFollow, Transform toFollow)
        {
            VRDFTransformFollower transFollower = transToFollow.AddComponent<VRDFTransformFollower>();
            transFollower.ToFollow = toFollow;
        }
    }
}