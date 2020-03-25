// -----------------------------------------------------------------------
// <copyright file="LoadBalancingTransport.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2015 Exit Games GmbH
// </copyright>
// <summary>
//   Extends Photon Realtime API with audio streaming functionality.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Realtime;

namespace Photon.Voice
{
    class VoiceEvent
    {
        /// <summary>
        /// Single event used for voice communications.
        /// </summary>
        /// Change if it conflicts with other event codes used in the same Photon room.
        public const byte Code = 202; // audio events had this code w
    }

    /// <summary>
    /// Extends LoadBalancingClient with audio streaming functionality.
    /// </summary>
    /// <remarks>
    /// Use your normal LoadBalancing workflow to join a Voice room. 
    /// All standard LoadBalancing features are available.
    ///
    /// To work with audio:
    /// - Create outgoing audio streams with Client.CreateLocalVoice.
    /// - Handle new incoming audio streams info with <see cref="OnRemoteVoiceInfoAction"/> .
    /// - Handle incoming audio streams data with <see cref="OnAudioFrameAction"/> .
    /// - Handle closing of incoming audio streams with <see cref="OnRemoteVoiceRemoved">.
    /// </remarks>
    public class LoadBalancingTransport : LoadBalancingClient, IVoiceTransport, IDisposable
    {
        const int VOICE_CHANNEL = 0;

        /// <summary>The <see cref="VoiceClient"></see> implementation associated with this LoadBalancingTransport.</summary>
        public VoiceClient VoiceClient { get { return this.voiceClient; } }

        protected VoiceClient voiceClient;
        private PhotonTransportProtocol protocol;

        public void LogError(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ERROR, string.Format(fmt, args)); }
        public void LogWarning(string fmt, params object[] args) { this.DebugReturn(DebugLevel.WARNING, string.Format(fmt, args)); }
        public void LogInfo(string fmt, params object[] args) { this.DebugReturn(DebugLevel.INFO, string.Format(fmt, args)); }
        public void LogDebug(string fmt, params object[] args) { this.DebugReturn(DebugLevel.ALL, string.Format(fmt, args)); }

        // send different media type to different channels for efficiency
        byte photonChannelForCodec(Codec c)
        {
            return (byte)(1 + Array.IndexOf(Enum.GetValues(typeof(Codec)), c));
        }

        public bool IsChannelJoined(int channelId) { return this.State == ClientState.Joined; }

        public void SetDebugEchoMode(LocalVoice v)
        {
            if (this.State == ClientState.Joined)
            {
                if (v.DebugEchoMode)
                {
                    SendVoicesInfo(new List<LocalVoice>() { v }, v.channelId, this.LocalPlayer.ActorNumber);
                }
                else
                {
                    SendVoiceRemove(v, v.channelId, this.LocalPlayer.ActorNumber);
                }
            }
        }

        /// <summary>
        /// Initializes a new <see cref="LoadBalancingTransport"/>.
        /// </summary>
        /// <param name="connectionProtocol">Connection protocol (UDP or TCP). <see cref="ConnectionProtocol"></see></param>
        public LoadBalancingTransport(ConnectionProtocol connectionProtocol = ConnectionProtocol.Udp) : base(connectionProtocol)
        {
            base.EventReceived += onEventActionVoiceClient;
            base.StateChanged += onStateChangeVoiceClient;
            this.voiceClient = new VoiceClient(this);
            var voiceChannelsCount = Enum.GetValues(typeof(Codec)).Length + 1; // channel per stream type, channel 0 is for user events
            if (LoadBalancingPeer.ChannelCount < voiceChannelsCount)
            {
                this.LoadBalancingPeer.ChannelCount = (byte)voiceChannelsCount;
            }
            this.protocol = new PhotonTransportProtocol(voiceClient, this);
        }

        /// <summary>
        /// This method dispatches all available incoming commands and then sends this client's outgoing commands.
        /// Call this method regularly (2 to 20 times a second).
        /// </summary>
        new public void Service()
        {
            base.Service();
            this.voiceClient.Service();
        }

        [Obsolete("Use LoadBalancingPeer::OpChangeGroups().")]
        public virtual bool ChangeAudioGroups(byte[] groupsToRemove, byte[] groupsToAdd)
        {
            return this.LoadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
        }

        [Obsolete("Use GlobalInterestGroup.")]
        public byte GlobalAudioGroup
        {
            get { return GlobalInterestGroup; }
            set { GlobalInterestGroup = value; }
        }
        /// <summary>
        /// Set global audio group for this client. This call sets InterestGroup for existing local voices and for created later to given value.
        /// Client set as listening to this group only until LoadBalancingPeer.OpChangeGroups() called. This method can be called any time.
        /// </summary>
        /// <see cref="LocalVoice.InterestGroup"/>
        /// <see cref="LoadBalancingPeer.OpChangeGroups(byte[], byte[])"/>
        public byte GlobalInterestGroup
        {
            get { return this.voiceClient.GlobalInterestGroup; }
            set
            {
                this.voiceClient.GlobalInterestGroup = value;
                if (this.State == ClientState.Joined)
                {
                    if (this.voiceClient.GlobalInterestGroup != 0)
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalInterestGroup });
                    }
                    else
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], null);
                    }
                }                
            }
        }


        #region nonpublic

        object sendLock = new object();

        //
        public void SendVoicesInfo(IEnumerable<LocalVoice> voices, int channelId, int targetPlayerId)
        {
            object content = protocol.buildVoicesInfo(voices, true);

            var sendOpt = new SendOptions()
            {
                Reliability = true,
            };

            var opt = new RaiseEventOptions();
            if (targetPlayerId != 0)
            {
                opt.TargetActors = new int[] { targetPlayerId };
            }
            lock (sendLock)
            {
                this.OpRaiseEvent(VoiceEvent.Code, content, opt, sendOpt);
            }

            if (targetPlayerId == 0) // send debug echo infos to myself if broadcast requested
            {
                SendDebugEchoVoicesInfo(channelId);
            }
        }

        /// <summary>Send VoicesInfo events to the local player for all voices that have DebugEcho enabled.</summary>
        /// This function will call <see cref="SendVoicesInfo"></see> for all local voices of our <see cref="VoiceClient"></see>
        /// that have DebugEchoMode set to true, with the given channel ID, and the local Player's ActorNumber as target.
        /// <param name="channelId">Transport Channel ID</param>
        public void SendDebugEchoVoicesInfo(int channelId)
        {
            var voices = voiceClient.LocalVoices.Where(x => x.DebugEchoMode);
            if (voices.Count() > 0)
            { 
                SendVoicesInfo(voices, channelId, this.LocalPlayer.ActorNumber);
            }
        }

        public void SendVoiceRemove(LocalVoice voice, int channelId, int targetPlayerId)
        {
            object content = protocol.buildVoiceRemoveMessage(voice);
            var sendOpt = new SendOptions()
            {
                Reliability = true,
            };

            var opt = new RaiseEventOptions();
            if (targetPlayerId != 0)
            {
                opt.TargetActors = new int[] { targetPlayerId };
            }
            if (voice.DebugEchoMode)
            {
                opt.Receivers = ReceiverGroup.All;
            }
            lock (sendLock)
            {

                this.OpRaiseEvent(VoiceEvent.Code, content, opt, sendOpt);
            }
        }

        public void SendFrame(ArraySegment<byte> data, byte evNumber, byte voiceId, int channelId, LocalVoice localVoice)
        {
            object[] content = protocol.buildFrameMessage(voiceId, evNumber, data);

            var sendOpt = new SendOptions()
            {
                Reliability = localVoice.Reliable,
                Channel = photonChannelForCodec(localVoice.info.Codec),
                Encrypt = localVoice.Encrypt
            };

            var opt = new RaiseEventOptions();
            if (localVoice.DebugEchoMode)
            {
                opt.Receivers = ReceiverGroup.All;
            }
            opt.InterestGroup = localVoice.InterestGroup;
            lock (sendLock)
            {
                this.OpRaiseEvent(VoiceEvent.Code, content, opt, sendOpt);
            }
            this.LoadBalancingPeer.SendOutgoingCommands();
        }

        public string ChannelIdStr(int channelId) { return null; }
        public string PlayerIdStr(int playerId) { return null; }

        private void onEventActionVoiceClient(EventData ev)
        {
            // check for voice event first
            if (ev.Code == VoiceEvent.Code)
            {
                // Payloads are arrays. If first array element is 0 than next is event subcode. Otherwise, the event is data frame with voiceId in 1st element.                    
                protocol.onVoiceEvent(ev[(byte)ParameterCode.CustomEventContent], VOICE_CHANNEL, (int)ev[ParameterCode.ActorNr], this.LocalPlayer.ActorNumber);
            }
            else
            {
                int playerId;
                switch (ev.Code)
                {
                    case (byte)EventCode.Join:
                        playerId = (int)ev[ParameterCode.ActorNr];
                        if (playerId == this.LocalPlayer.ActorNumber)
                        {
                        }
                        else
                        {
                            this.voiceClient.onPlayerJoin(VOICE_CHANNEL, playerId);                            
                        }
                        break;
                    case (byte)EventCode.Leave:
                        {
                            playerId = (int)ev[ParameterCode.ActorNr];
                            if (playerId == this.LocalPlayer.ActorNumber)
                            {
                                this.voiceClient.onLeaveAllChannels();
                            }
                            else
                            {
                                this.voiceClient.onPlayerLeave(VOICE_CHANNEL, playerId);
                            }
                        }
                        break;
                }
            }
        }

        void onStateChangeVoiceClient(ClientState fromState, ClientState state)
        {
            switch (fromState)
            {
                case ClientState.Joined:
                    this.voiceClient.onLeaveChannel(VOICE_CHANNEL);
                    break;
            }

            switch (state)
            {
                case ClientState.Joined:
                    this.voiceClient.onJoinChannel(VOICE_CHANNEL);
                    if (this.voiceClient.GlobalInterestGroup != 0)
                    {
                        this.LoadBalancingPeer.OpChangeGroups(new byte[0], new byte[] { this.voiceClient.GlobalInterestGroup });
                    }
                    break;
            }
        }        

        #endregion

        /// <summary>
        /// Releases all resources used by the <see cref="LoadBalancingTransport"/> instance.
        /// </summary>
        public void Dispose()
        {
            this.voiceClient.Dispose();
        }
    }
}
