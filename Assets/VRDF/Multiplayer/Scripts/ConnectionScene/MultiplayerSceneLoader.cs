using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Load the Multiplayer scene on room created
    /// </summary>
    public class MultiplayerSceneLoader : MonoBehaviourPunCallbacks
    {
        [Tooltip("Should we load the scene as soon as the room was created ?")]
        [SerializeField] private bool _loadSceneOnRoomCreated = true;

        /// <summary>
        /// The name or index of the scene you want to load as a multiplayer scene. 
        /// </summary>
        [Tooltip("The name of the scene you want to load as a multiplayer scene.")]
        public string MultiplayerSceneName = "MultiplayerScene";

        /// <summary>
        /// Callback for when a room was correctly created.
        /// </summary>
        public override void OnCreatedRoom()
        {
            if (_loadSceneOnRoomCreated && !TryToLoadMultiplayerScene())
            {
                VRDF_Components.DebugVRDFMessage("Can't load the Multiplayer Scene. Check the name and index of your multiplayer scene, and be sure that this scene was added in the Build Settings. Stopping app.", true);
                Application.Quit();
            }
        }

        /// <summary>
        /// Try to load a scene based on its name
        /// </summary>
        /// <returns>true if the scene was correctly loaded</returns>
        public bool TryToLoadMultiplayerScene()
        {
            try
            {
                VRDF_Components.DebugVRDFMessage("Trying to load the scene with name '{0}'.", debugParams: MultiplayerSceneName);
                PhotonNetwork.LoadLevel(MultiplayerSceneName);
                return true;
            }
            catch
            {
                VRDF_Components.DebugVRDFMessage("Couldn't load scene with name '{0}'.", true, MultiplayerSceneName);
                return false;
            }
        }
    }
}