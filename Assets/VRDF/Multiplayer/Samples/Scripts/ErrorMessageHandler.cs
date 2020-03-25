using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// 
    /// </summary>
    public class ErrorMessageHandler : MonoBehaviourPunCallbacks
    {
        [Tooltip("The panel to tell the tuser there's an error.")]
        [SerializeField]
        private GameObject _errorPanel;

        /// <summary>
        /// Callback for when we couldn't create a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            _errorPanel.SetActive(true);
        }

        /// <summary>
        /// Callback for when we couldn't join a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            _errorPanel.SetActive(true);
        }

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            _errorPanel.SetActive(true);
        }
    }
}