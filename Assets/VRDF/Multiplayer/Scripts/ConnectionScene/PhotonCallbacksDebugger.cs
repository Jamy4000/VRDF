using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Used to display some messages in the console whenever we fetch a callback from Photon
    /// </summary>
    public class PhotonCallbacksDebugger : MonoBehaviourPunCallbacks
    {
        [Tooltip("If set at false, no message coming from this script will be displayed in the console")]
        [SerializeField] private bool _showConsoleMessages = true;

        /// <summary>
        /// Callback for when the user could connect to the server.
        /// </summary>
        public override void OnConnectedToMaster()
        {
            DebugMessage("<Color=Green>Connection with Master Server established !</Color>");
        }

        /// <summary>
        /// Callback for when the user could connect to the server.
        /// </summary>
        public override void OnJoinedLobby()
        {
            DebugMessage("<Color=Green>Lobby was successfully joined !</Color>");
            DebugMessage("{0} players, including this instance of the game, are currently online in your app.", debugParams: PhotonNetwork.CountOfPlayers);
            DebugMessage("{0} players, including this instance of the game, are currently looking for a room.", debugParams: PhotonNetwork.CountOfPlayersOnMaster);
            DebugMessage("{0} players are currently inside a room.", debugParams: PhotonNetwork.CountOfPlayersInRooms);
            DebugMessage("{0} rooms are currently online.", debugParams: PhotonNetwork.CountOfRooms);
        }

        /// <summary>
        /// Callback for when a room was correctly created.
        /// </summary>
        public override void OnCreatedRoom()
        {
            DebugMessage("<Color=Green>The room was successfully <b>CREATED</b> !</Color>");
        }

        /// <summary>
        /// Callback for when we couldn't create a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            DebugMessage("The room couldn't be <b>CREATED</b>. Here's the return code :\n{0}.\nAnd here's the message :\n{1}.", true, returnCode, message);
            switch (returnCode)
            {
                case 0x7FFF - 1:
                    DebugMessage("A room with this name already exist. Please Join Lobby to access the available rooms.", true);
                    break;
            }
        }

        /// <summary>
        /// Callback for when a room was correctly joined.
        /// </summary>
        public override void OnJoinedRoom()
        {
            DebugMessage("<Color=Green>The room was successfully <b>JOINED</b>.</Color>");

            // Only display the rest if we automatically sync the scenes, and if the player isn't in the connection scene anymore
            if (PhotonNetwork.AutomaticallySyncScene && VRDFConnectionManager.Instance != null)
                DebugMessage("Loading the current Room's scene...");
        }

        /// <summary>
        /// Callback for when we couldn't join a room for any reason.
        /// </summary>
        /// <param name="returnCode">The error code returned by photon</param>
        /// <param name="message">The error message returned by photon and describing the problem</param>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            DebugMessage("The room couldn't be <b>JOINED</b>. Here's the return code :\n{0}.\nAnd here's the message :\n{1}.", true, returnCode, message);
            
            switch (returnCode)
            {
                case 0x7FFF - 7:
                    DebugMessage("No random room found.", true);
                    break;
                case 0x7FFF - 9:
                    DebugMessage("Room doesn't exist anymore.", true);
                    break;
            }
        }

        /// <summary>
        /// Called when a Photon Player got connected.
        /// </summary>
        /// <param name="other">The player that just entered the room</param>
        public override void OnPlayerEnteredRoom(Player other)
        {
            DebugMessage("The player with name {0} has <b>ENTER</b> the room.", debugParams: other.NickName);
        }

        /// <summary>
        /// Called when a Photon Player got disconnected.
        /// </summary>
        /// <param name="other">The player that just left the room</param>
        public override void OnPlayerLeftRoom(Player other)
        {
            DebugMessage("The player with name {0} has <b>LEFT</b> the room.", debugParams: other.NickName);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            DebugMessage("You've been disconnected ! Disconnection cause={0},\n ClientState={1},\n PeerState={2}",
                debugParams: new object[3] { cause,
                PhotonNetwork.NetworkingClient.State,
                PhotonNetwork.NetworkingClient.LoadBalancingPeer.PeerState });
        }

        /// <summary>
        /// Only check if the _showConsoleMessages is set at true, and if so, display a message in the Console.
        /// </summary>
        /// <param name="message">The message to display in the console</param>
        /// <param name="isErrorMessage">Is this message supposed to be an error ?</param>
        /// <param name="debugParams">the parameters to put at the end of the Debug.LogFormat</param>
        private void DebugMessage(string message, bool isErrorMessage = false, params object[] debugParams)
        {
            if (_showConsoleMessages)
                VRDF_Components.DebugVRDFMessage(message, isErrorMessage, debugParams);
        }
    }
}