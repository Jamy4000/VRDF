using Photon.Pun;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Keep track of the local player and of all other players in the room
    /// </summary>
    public class VRDFPlayerManager : MonoBehaviourPunCallbacks
    {
        private VRDFPlayer _thisPlayer;

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
            _thisPlayer = new VRDFPlayer(photonView.Owner, gameObject);
            VRDFPlayerUtilities.PlayersInstances.Add(_thisPlayer);

            if (photonView.IsMine)
            {
                VRDFPlayerUtilities.LocalVRDFPlayer = _thisPlayer;
                LocalPlayerGameObjectInstance = gameObject;
            }
        }

        protected virtual void OnDestroy()
        {
            VRDFPlayerUtilities.PlayersInstances.Remove(_thisPlayer);
        }
    }
}