using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Base class for unexpected disconnects recovery. 
    /// Based on this thread : https://forum.photonengine.com/discussion/14081/how-to-reconnect-and-rejoin-when-mobile-app-goes-to-background-best-practice
    /// Check the response of Jeanfabre mod, and its link to this script : https://pastebin.com/wk39tgzA
    /// </summary>
    public abstract class DisconnectionRecoveryHandler : MonoBehaviourPunCallbacks
    {
        [Header("Should we try to rejoin/reconnect everytime it fails ?")]
        [Tooltip("If set at false, we will try to reconnect/rejoin one time, and if it's not successful, the user will be send back to the Connection Scene")]
        public bool RetryToReconnectOnFail = true;

        /// <summary>
        /// Did we already try to rejoin the room / Reconnect to the server after being disconnected ?
        /// </summary>
        [HideInInspector] protected bool TriedToReconnect;

        private void Awake()
        {
            // More for mobile platform like Oculus Quest, when the user hasn't is head in the headset anymore
            PhotonNetwork.KeepAliveInBackground = 60.0f;
            PhotonNetwork.PhotonServerSettings.RunInBackground = true;
            Application.runInBackground = true;
        }

        protected abstract void HandleDisconnect(DisconnectCause cause);

        public abstract void TryToReconnect();

        public abstract bool OnReconnectionFailed();
    }
}