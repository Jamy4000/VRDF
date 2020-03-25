using System.Threading;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Try to reconnect to a room if an unexpected disconnection happen. 
    /// Based on this thread : https://forum.photonengine.com/discussion/14081/how-to-reconnect-and-rejoin-when-mobile-app-goes-to-background-best-practice
    /// Check the response of Jeanfabre mod, and its link to this script : https://pastebin.com/wk39tgzA
    /// </summary>
    public class RoomConnectionRecoverer : DisconnectionRecoveryHandler
    {
        /// <summary>
        /// How long before a player is kicked out of a room
        /// </summary>
        private float _playerTtlInRoom = -1.0f;

        /// <summary>
        /// The game manager of the room, to send back the user to the connection room if needed
        /// </summary>
        private VRDFMultiplayerGameManager _gameManager;

        /// <summary>
        /// CancellationTokenSource to cancel waiting Task
        /// </summary>
        private CancellationTokenSource _tokenSource;

        /// <summary>
        /// CancellationToken fetched from the CancellationTokenSource
        /// </summary>
        private CancellationToken _token;

        /// <summary>
        /// Whether we're currently waiting to send back the user in the Connection Room, aka the Task is running in the background
        /// </summary>
        private bool _isWaitingForTask;

        private void Start()
        {
            _playerTtlInRoom = PhotonNetwork.CurrentRoom.PlayerTtl;
            _gameManager = VRDFMultiplayerGameManager.Instance;
        }

        /// <summary>
        /// Whenever the user is disconnected from the room and he already tried to rejoin, 
        /// we either try one more time or send him back to the connection scene.
        /// </summary>
        /// <returns>Whether or not we can start the DisconnectionWaitingThread when exiting this method.</returns>
        public override bool OnReconnectionFailed()
        {
            VRDF_Components.DebugVRDFMessage("Rejoin has failed, the client is still disconnected.", true);

            if (RetryToReconnectOnFail)
            {
                VRDF_Components.DebugVRDFMessage("Trying to rejoin one more time.");
                TriedToReconnect = PhotonNetwork.ReconnectAndRejoin();
                return true;
            }
            else
            {
                SendBackUserToConnectionRoom();
                return false;
            }
        }

        /// <summary>
        /// Try to bring back the user in the room if the player time out isn't 0
        /// </summary>
        public override void TryToReconnect()
        {
            if (_playerTtlInRoom > 0.0f)
                TriedToReconnect = PhotonNetwork.ReconnectAndRejoin();
            else
                SendBackUserToConnectionRoom();
        }

        /// <summary>
        /// Cancel the waiting Task, and send back the user to the connection scene
        /// </summary>
        private void SendBackUserToConnectionRoom()
        {
            if (_tokenSource != null)
                _tokenSource.Cancel(true);

            _gameManager.GoBackToConnectionScene();
        }

        /// <summary>
        /// Photon callback for when the room was rejoined
        /// </summary>
        public override void OnJoinedRoom()
        {
            _tokenSource?.Cancel(true);

            if (TriedToReconnect)
            {
                VRDF_Components.DebugVRDFMessage("<Color=green>Rejoin successful !</Color>");
                TriedToReconnect = false;
            }
        }

        /// <summary>
        /// Photon callback for when the room couldn't be rejoined
        /// </summary>
        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            // When we tried for the first time to rejoin, we just don't care as OnDisconnected is gonna be raised as well
            if (!TriedToReconnect)
            {
                return;
            }
            // Room doesn't exist anymore
            else if (returnCode == 0x7FFF - 9)
            {
                SendBackUserToConnectionRoom();
                return;
            }

            TriedToReconnect = false;
            OnReconnectionFailed();
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            bool canStartWaitingThread = true;

            // If we already tried to rejoin the current room
            if (TriedToReconnect)
                canStartWaitingThread = OnReconnectionFailed();

            // If we didn't tried anything and the current client is disconnected
            else if (PhotonNetwork.NetworkingClient.State == ClientState.Disconnected)
                this.HandleDisconnect(cause);

            // We start a side thread waiting until the player timeOut is passed, and send back the user to the connection scene.
            if (canStartWaitingThread && _playerTtlInRoom > 0.0f && cause != DisconnectCause.DisconnectByClientLogic && !_isWaitingForTask)
            {
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;
                StartDisconnectionWaitingThread();
            }
        }

        /// <summary>
        /// Async method waiting for a task to end or for the cancellation tokens to be set at true
        /// Once done, if the token hasn't change, it means the player hasn't been able to reconnect after Player Timeout time, so we load the connectionScene
        /// </summary>
        private async void StartDisconnectionWaitingThread()
        {
            _isWaitingForTask = true;

            try
            {
                bool shouldLoadConnectionScene = true;

                await Task.Run(() => {
                    _token.Register(() => shouldLoadConnectionScene = false);
                    Task.Delay((int)_playerTtlInRoom).Wait();
                }, _token);

                if (shouldLoadConnectionScene && Application.isPlaying)
                    SendBackUserToConnectionRoom();
            }
            catch { }

            _isWaitingForTask = false;
        }
    }
}