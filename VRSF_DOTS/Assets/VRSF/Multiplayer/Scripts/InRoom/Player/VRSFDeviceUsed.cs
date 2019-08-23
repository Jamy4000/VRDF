using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Multiplayer
{
    /// <summary>
    /// Used to get and send which VR Device is used by our current users
    /// </summary>
    public class VRSFDeviceUsed : MonoBehaviourPunCallbacks
    {
        /// <summary>
        /// As the players in VRSFPlayersManager may sometimes take some times to be updated, we keep track of the one we need to update
        /// using this dictionary.
        /// </summary>
        private Dictionary<Player, EDevice> _playersToSet = new Dictionary<Player, EDevice>();

        private void Awake()
        {
            OnSetupVRReady.RegisterSetupVRResponse(SetupVRReady);
        }

        private void Update()
        {
            // If we need to update one of the player
            if (_playersToSet.Count > 0)
            {
                List<Player> toRemove = new List<Player>(); 
                foreach (var kvp in _playersToSet)
                {
                    try
                    {
                        var player = VRSFBasicPlayersManager.FindPlayer(kvp.Key, false);
                        player.DeviceUsed = kvp.Value;
                        toRemove.Add(kvp.Key);

                        new VRDeviceWasSet(player);
                    }
                    catch
                    {
                        Debug.LogFormat("<b>[VRSF] :</b> Player {0} is still not set in the VRSFPlayersManager.PlayersInstances. Waiting for next Frame.", kvp.Key.NickName);
                    }
                }

                // We destroy all the players that were correctly set from the dictionary
                foreach (var player in toRemove)
                    _playersToSet.Remove(player);
            }
        }

        private void OnDestroy()
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(SetupVRReady))
                OnSetupVRReady.Listeners -= SetupVRReady;
        }

        /// <summary>
        /// Callback for whenever a new user enter the room. Ask everyone except me to send their VR devices
        /// </summary>
        /// <param name="newPlayer">The player that entered the room</param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            photonView.RPC("SendDeviceInfo", RpcTarget.Others, PhotonNetwork.LocalPlayer, VRSF_Components.DeviceLoaded);
            base.OnPlayerEnteredRoom(newPlayer);
        }

        /// <summary>
        /// Callback for whenever SetupVR is done. Ask everyone except me to send their VR devices
        /// </summary>
        private void SetupVRReady(OnSetupVRReady info)
        {
            photonView.RPC("SendDeviceInfo", RpcTarget.Others, PhotonNetwork.LocalPlayer, VRSF_Components.DeviceLoaded);
        }

        [PunRPC]
        private void SendDeviceInfo(Player sender, EDevice deviceUsed)
        {
            // Security as this method is called OnSetupVRReady and OnPlayerEnteredRoom
            if (!_playersToSet.ContainsKey(sender))
                _playersToSet.Add(sender, deviceUsed);
        }
    }
}