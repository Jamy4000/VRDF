using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// used to connect the user to the Lobby and display available rooms afterwards
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
            Debug.LogErrorFormat("<Color=Red><b>[VRSF] :</b> The room couldn't be CREATED. Here's the return code :</Color>\n{0}.<Color=Red>\nAnd here's the message :</Color>\n{1}.", returnCode, message);
            _errorPanel.SetActive(true);
        }

        /// <summary>
        /// Callback for when we couldn't join a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogErrorFormat("<Color=Red><b>[VRSF] :</b> The room couldn't be JOINED. Here's the return code :\n{0}.\nAnd here's the message :\n{1}.</Color>", returnCode, message);
            _errorPanel.SetActive(true);
        }

        public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
        {
            Debug.LogFormat("<b>[VRSF]:</b> {0} players are currently online in your app.", PhotonNetwork.CountOfPlayers);
            _errorPanel.SetActive(true);
        }
    }
}