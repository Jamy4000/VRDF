using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Game manager.
    /// Connects and watch Photon Status, Instantiate Player
    /// Deals with quiting the room and the game
    /// Deals with level loading (outside the in room synchronization)
    /// </summary>
    public class VRDFMultiplayerGameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("The name of the Connection Scene")]
        [SerializeField]
        private string _connectionSceneName = "ConnectionScene";

        [Tooltip("Whether we send back the user in the connection Scene when he leave the room (called as well when user is disconnected)")]
        [SerializeField]
        private bool _sendBackInConnectionSceneOnUserLeft;

        public static VRDFMultiplayerGameManager Instance;

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                VRDF_Components.DebugVRDFMessage("Another instance of VRDFConnectionManager exist. Be sure to only have one VRDFConnectionManager in your Scene.", true);
                Destroy(this);
                return;
            }

            Instance = this;
        }

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        protected virtual void Start()
        {
            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                VRDF_Components.DebugVRDFMessage("You're not connected to the Photon Network ! Sending back to Connection Scene.", true);
                SceneManager.LoadScene(_connectionSceneName);
            }
        }

        protected virtual void OnDestro()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Called when the local player leave the room, send him back to the connection scene
        /// </summary>
        public override void OnLeftRoom()
        {
            if (_sendBackInConnectionSceneOnUserLeft)
                GoBackToConnectionScene();
        }

        public void GoBackToConnectionScene()
        {
            SceneManager.LoadScene(_connectionSceneName);
        }
    }
}