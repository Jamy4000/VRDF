using Photon.Pun;
using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Keep track of the local player and of all other players in the room
    /// </summary>
    public class VRSFPlayerManager : MonoBehaviourPunCallbacks
    {
        private VRSFPlayer _thisPlayer;

        /// <summary>
        /// The local player instance. 
        /// Use this to get the local player gameObject in DontDestroyOnLoad.
        /// </summary>
        public static GameObject LocalPlayerGameObjectInstance;

        protected virtual void Awake()
        {
            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        protected virtual void Start()
        {
            _thisPlayer = new VRSFPlayer(photonView.Owner, gameObject);
            VRSFPlayerUtilities.PlayersInstances.Add(_thisPlayer);

            if (photonView.IsMine)
            {
                VRSFPlayerUtilities.LocalVRSFPlayer = _thisPlayer;
                LocalPlayerGameObjectInstance = gameObject;
            }
        }

        protected virtual void OnDestroy()
        {
            VRSFPlayerUtilities.PlayersInstances.Remove(_thisPlayer);
        }
    }
}