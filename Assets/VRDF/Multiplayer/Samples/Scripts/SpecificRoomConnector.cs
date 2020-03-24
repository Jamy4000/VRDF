using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

namespace VRDF.Multiplayer.Samples
{
    /// <summary>
    /// Create a request to join a room when knowing its specific name
    /// </summary>
    public class SpecificRoomConnector : MonoBehaviourPunCallbacks
    {
        [Header("Objects to Join the Lobby")]
        [SerializeField] private UnityEngine.UI.Button _enterRoomButton;

        private string _roomToEnter;

        /// <summary>
        /// Called from the Enter Room button
        /// </summary>
        public void JoinRoom()
        {
            PhotonNetwork.JoinRoom(_roomToEnter);
        }

        public void OnNewRoomNameEntered(string newName)
        {
            _roomToEnter = newName;
            _enterRoomButton.interactable = !string.IsNullOrEmpty(_roomToEnter);
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            _enterRoomButton.interactable = !string.IsNullOrEmpty(_roomToEnter);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            _enterRoomButton.interactable = false;
        }
    }
}