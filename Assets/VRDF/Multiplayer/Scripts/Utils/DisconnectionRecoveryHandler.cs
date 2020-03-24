using System;
using System.Threading;
using System.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Unexpected disconnects recovery. 
    /// Based on this thread : https://forum.photonengine.com/discussion/14081/how-to-reconnect-and-rejoin-when-mobile-app-goes-to-background-best-practice
    /// Check the response of Jeanfabre mod, and its link to this script : https://pastebin.com/wk39tgzA
    /// </summary>
    public class DisconnectionRecoveryHandler : MonoBehaviourPunCallbacks
    {
        [Header("Should we try to rejoin/reconnect everytime it fails ?")]
        [Tooltip("If set at false, we will try to reconnect/rejoin one time, and if it's not successful, the user will be send back to the Connection Scene")]
        [SerializeField]
        private bool _retryToRejoinOnJoinFailed = true;

        /// <summary>
        /// Did we already try to rejoin the room after being disconnected ?
        /// </summary>
        private bool _rejoinCalled;
        /// <summary>
        /// Did we already try to reconnect to the server after being disconnected ?
        /// </summary>
        private bool _reconnectCalled;

        /// <summary>
        /// Was the user in a room when he got disconnected ?
        /// </summary>
        private bool _wasInRoom;

        /// <summary>
        /// How long before a player is kicked out of a room
        /// </summary>
        private float _playerTtlInRoom = -1.0f;

        /// <summary>
        /// Reference to the VRDFMultiplayerGameManager. Can be null if this script is placed in the Connection Scene.
        /// </summary>
        private VRDFMultiplayerGameManager _gameManager;
        
        private CancellationTokenSource _tokenSource;
        
        private CancellationToken _token;

        private bool _isWaitingForTask;

        private void Awake()
        {
            // More for mobile platform like Oculus Quest, when the user hasn't is head in the headset anymore
            PhotonNetwork.KeepAliveInBackground = 60.0f;
            PhotonNetwork.PhotonServerSettings.RunInBackground = true;
            Application.runInBackground = true;
        }

        private void Start()
        {
            _gameManager = VRDFMultiplayerGameManager.Instance;
            // no playerTtl in ConnectionScene, so if VRDFMultiplayerGameManager Instance is null, we're in the ConectionScene
            _playerTtlInRoom = _gameManager == null ? 0.0f : PhotonNetwork.CurrentRoom.PlayerTtl;
            _wasInRoom = _gameManager != null;
        }

        #region Photon_Callbacks
        public override void OnDisconnected(DisconnectCause cause)
        {
            bool canStartWaitingThread = true;
            
            // If we already tried to rejoin the current room
            if (_rejoinCalled)
                canStartWaitingThread = HandleRejoinCalled();

            // If we already try to reconnect to the Photon Servers
            else if (_reconnectCalled)
                canStartWaitingThread = HandleReconnectCalled();

            // If we didn't tried anything and the current client is disconnected
            else if (PhotonNetwork.NetworkingClient.State == ClientState.Disconnected)
                this.HandleDisconnect(cause);

            // We start a side thread waiting until the player timeOut is passed, and send back the user to the connection scene.
            if (canStartWaitingThread && _playerTtlInRoom != 0.0f && cause != DisconnectCause.DisconnectByClientLogic && !_isWaitingForTask)
            {
                _tokenSource = new CancellationTokenSource();
                _token = _tokenSource.Token;
                StartDisconnectionWaitingThread();
            }
        }

        /// <summary>
        /// Whenever the user is disconnected from the Server and he already tried to reconnect, 
        /// we either try one more time or send him back to the connection scene if he isn't there already.
        /// </summary>
        /// <returns>Whether or not we can start the DisconnectionWaitingThread when exiting this method.</returns>
        private bool HandleReconnectCalled()
        {
            VRDF_Components.DebugVRDFMessage("Reconnect failed, client disconnected.", true);

            if (_retryToRejoinOnJoinFailed)
            {
                VRDF_Components.DebugVRDFMessage("Trying to reconnect one more time.");
                _reconnectCalled = PhotonNetwork.Reconnect();
                return true;
            }
            else
            {
                SendBackUserToLobby();
                return false;
            }
        }

        /// <summary>
        /// Whenever the user is disconnected from the room and he already tried to rejoin, 
        /// we either try one more time or send him back to the connection scene.
        /// </summary>
        /// <returns>Whether or not we can start the DisconnectionWaitingThread when exiting this method.</returns>
        private bool HandleRejoinCalled()
        {
            VRDF_Components.DebugVRDFMessage("Rejoin failed, client disconnected;", true);

            if (_retryToRejoinOnJoinFailed)
            {
                VRDF_Components.DebugVRDFMessage("Trying to rejoin one more time.");
                _rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                return true;
            }
            else
            {
                SendBackUserToLobby();
                return false;
            }
        }

        /// <summary>
        /// Abort the waiting thread, set it to null, and 
        /// </summary>
        private void SendBackUserToLobby()
        {
            _tokenSource?.Cancel(true);
            // if GameManager is null, we're already in the ConnectionScene anyway
            _gameManager?.GoBackToConnectionScene();
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            if (!_rejoinCalled)
            {
                return;
            }
            // Room doesn't exist anymore
            else if (returnCode == 0x7FFF - 9)
            {
                SendBackUserToLobby();
                return;
            }

            _rejoinCalled = false;
            VRDF_Components.DebugVRDFMessage("Quick rejoin failed.", true);

            if (_retryToRejoinOnJoinFailed)
            {
                VRDF_Components.DebugVRDFMessage("Trying to rejoin one more time");
                _rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
            }
            else
            {
                SendBackUserToLobby();
            }
        }

        public override void OnJoinedRoom()
        {
            _tokenSource?.Cancel(true);

            if (_rejoinCalled)
            {
                VRDF_Components.DebugVRDFMessage("<Color=green>Rejoin successful !</Color>");
                _rejoinCalled = false;
            }
        }

        public override void OnConnectedToMaster()
        {
            _tokenSource?.Cancel(true);

            if (_reconnectCalled)
            {
                VRDF_Components.DebugVRDFMessage("<Color=green>Reconnection successful !</Color>");
                _reconnectCalled = false;
            }
        }
        #endregion Photon_Callbacks

        private void HandleDisconnect(DisconnectCause cause)
        {
            VRDF_Components.DebugVRDFMessage("Trying to rejoin the server ...");

            switch (cause)
            {
                case DisconnectCause.Exception:
                case DisconnectCause.ServerTimeout:
                case DisconnectCause.ClientTimeout:
                case DisconnectCause.DisconnectByServerLogic:
                case DisconnectCause.AuthenticationTicketExpired:
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    if (_wasInRoom)
                    {
                        if (_playerTtlInRoom > 0.0f)
                            _rejoinCalled = PhotonNetwork.ReconnectAndRejoin();
                        else
                            SendBackUserToLobby();
                    }
                    else
                    {
                        _reconnectCalled = PhotonNetwork.Reconnect();
                    }
                    break;
                case DisconnectCause.OperationNotAllowedInCurrentState:
                case DisconnectCause.CustomAuthenticationFailed:
                case DisconnectCause.DisconnectByClientLogic:
                case DisconnectCause.InvalidAuthentication:
                case DisconnectCause.ExceptionOnConnect:
                case DisconnectCause.MaxCcuReached:
                case DisconnectCause.InvalidRegion:
                case DisconnectCause.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("cause", cause, "cause not supported");
            }
        }

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
                    _gameManager?.GoBackToConnectionScene();
            }
            catch { }
        }
    }
}