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

        protected virtual void Start()
        {
            // in case we started this demo with the wrong scene being active, simply load the connection scene
            if (!PhotonNetwork.IsConnectedAndReady)
            {
                VRDF_Components.DebugVRDFMessage("You're not connected to the Photon Network ! Sending back to Connection Scene.", true);
                SceneManager.LoadScene(_connectionSceneName);
            }
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void GoBackToConnectionScene()
        {
            SceneManager.LoadScene(_connectionSceneName);
        }
    }
}