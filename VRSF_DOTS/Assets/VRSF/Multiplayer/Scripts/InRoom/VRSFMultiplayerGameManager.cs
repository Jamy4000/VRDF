using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Game manager.
    /// Connects and watch Photon Status, Instantiate Player
    /// Deals with quiting the room and the game
    /// Deals with level loading (outside the in room synchronization)
    /// </summary>
    public class VRSFMultiplayerGameManager : MonoBehaviourPunCallbacks
    {
        [Header("The name of the Lobby Scene")]
        [SerializeField]
        private string _lobbySceneName = "BasicVRMultiLobby";

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start()
        {
            // in case we started this demo with the wrong scene being active, simply load the menu scene
            if (!PhotonNetwork.IsConnectedAndReady)
                SceneManager.LoadScene(_lobbySceneName);
        }

        /// <summary>
        /// Called when a Photon Player got connected. We need to then load a bigger scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.Log("<b>[VRSF] :</b> OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting
        }

        /// <summary>
        /// Called when a Photon Player got disconnected. We need to load a smaller scene.
        /// </summary>
        /// <param name="other">Other.</param>
        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.Log("<b>[VRSF] :</b> OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects
        }

        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(_lobbySceneName);
        }
    }
}