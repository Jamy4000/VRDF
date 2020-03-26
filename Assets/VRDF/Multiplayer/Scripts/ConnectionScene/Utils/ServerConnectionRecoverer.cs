using Photon.Pun;
using Photon.Realtime;

namespace VRDF.Multiplayer
{
    /// <summary>
    /// Used in Connection Scene, try to reconnect to the server if an unexpected disconnection happen. 
    /// Based on this thread : https://forum.photonengine.com/discussion/14081/how-to-reconnect-and-rejoin-when-mobile-app-goes-to-background-best-practice
    /// Check the response of Jeanfabre mod, and its link to this script : https://pastebin.com/wk39tgzA
    /// </summary>
    public class ServerConnectionRecoverer : DisconnectionRecoveryHandler
    {
        /// <summary>
        /// Whenever the user is disconnected from the Server and he already tried to reconnect, 
        /// we either try one more time or send him back to the connection scene if he isn't there already.
        /// </summary>
        /// <returns>Whether or not we try to reconnect again when existing this method</returns>
        public override bool OnReconnectionFailed()
        {
            VRDF_Components.DebugVRDFMessage("Reconnect failed, client disconnected.", true);

            if (RetryToReconnectOnFail)
            {
                VRDF_Components.DebugVRDFMessage("Trying to reconnect one more time.");
                TriedToReconnect = PhotonNetwork.Reconnect();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void TryToReconnect()
        {
            TriedToReconnect = PhotonNetwork.Reconnect();
        }

        public override void OnConnectedToMaster()
        {
            if (TriedToReconnect)
            {
                VRDF_Components.DebugVRDFMessage("<Color=green>Reconnection successful !</Color>");
                TriedToReconnect = false;
            }
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            // If we already try to reconnect to the Photon Servers
            if (TriedToReconnect)
                OnReconnectionFailed();

            // If we didn't tried anything and the current client is disconnected
            else if (PhotonNetwork.NetworkingClient.State == ClientState.Disconnected)
                HandleDisconnect(cause);
        }

        protected override void HandleDisconnect(DisconnectCause cause)
        {
            switch (cause)
            {
                case DisconnectCause.AuthenticationTicketExpired:
                case DisconnectCause.DisconnectByServerReasonUnknown:
                    VRDF_Components.DebugVRDFMessage("Trying to rejoin the server ...");
                    TryToReconnect();
                    break;
                case DisconnectCause.ServerTimeout:
                case DisconnectCause.ClientTimeout:
                case DisconnectCause.Exception:
                case DisconnectCause.ExceptionOnConnect:
                case DisconnectCause.OperationNotAllowedInCurrentState:
                case DisconnectCause.CustomAuthenticationFailed:
                case DisconnectCause.DisconnectByClientLogic:
                case DisconnectCause.InvalidAuthentication:
                case DisconnectCause.MaxCcuReached:
                case DisconnectCause.InvalidRegion:
                case DisconnectCause.None:
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException("cause", cause, "cause not supported");
            }
        }
    }
}