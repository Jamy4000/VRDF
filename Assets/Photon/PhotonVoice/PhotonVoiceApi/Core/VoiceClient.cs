﻿// -----------------------------------------------------------------------
// <copyright file="VoiceClient.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
//#define PHOTON_VOICE_VIDEO_ENABLE
using System;
using System.Linq;
using System.Collections.Generic;
namespace Photon.Voice
{
    public interface ILogger
    {
        void LogError(string fmt, params object[] args);
        void LogWarning(string fmt, params object[] args);
        void LogInfo(string fmt, params object[] args);
        void LogDebug(string fmt, params object[] args);
    }
    interface IVoiceTransport : ILogger
    {
        bool IsChannelJoined(int channelId);
        void SendVoicesInfo(IEnumerable<LocalVoice> voices, int channelId, int targetPlayerId);
        void SendVoiceRemove(LocalVoice voice, int channelId, int targetPlayerId);
        void SendFrame(ArraySegment<byte> data, byte evNumber, byte voiceId, int channelId, LocalVoice localVoice);
        string ChannelIdStr(int channelId);
        string PlayerIdStr(int playerId);
        void SetDebugEchoMode(LocalVoice v);
    }
    /// <summary>
    /// Voice client interact with other clients on network via IVoiceTransport.
    /// </summary>        
    public class VoiceClient : IDisposable
    {
        internal IVoiceTransport transport;
        /// <summary>Lost frames counter.</summary>
        public int FramesLost { get; internal set; }
        /// <summary>Received frames counter.</summary>
        public int FramesReceived { get; private set; }
        /// <summary>Sent frames counter.</summary>
        public int FramesSent { get { int x = 0; foreach (var v in this.localVoices) { x += v.Value.FramesSent; } return x; } }
        /// <summary>Sent frames bytes counter.</summary>
        public int FramesSentBytes { get { int x = 0; foreach (var v in this.localVoices) { x += v.Value.FramesSentBytes; } return x; } }
        /// <summary>Average time required voice packet to return to sender.</summary>
        public int RoundTripTime { get; private set; }
        /// <summary>Average round trip time variation.</summary>
        public int RoundTripTimeVariance { get; private set; }
        /// <summary>Do not log warning when duplicate info received.</summary>
        public bool SuppressInfoDuplicateWarning { get; set; }
        /// <summary>Remote voice info event delegate.</summary>        
        public delegate void RemoteVoiceInfoDelegate(int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options);
        /// <summary>
        /// Register a method to be called when remote voice info arrived (after join or new new remote voice creation).
        /// Metod parameters: (int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options);
        /// </summary>
        public RemoteVoiceInfoDelegate OnRemoteVoiceInfoAction { get; set; }
        /// <summary>Lost frames simulation ratio.</summary>
        public int DebugLostPercent { get; set; }
        private int prevRtt = 0;
        /// <summary>Iterates through copy of all local voices list.</summary>
        public IEnumerable<LocalVoice> LocalVoices
        {
            get
            {
                var res = new LocalVoice[this.localVoices.Count];
                this.localVoices.Values.CopyTo(res, 0);
                return res;
            }
        }
        /// <summary>Iterates through copy of all local voices list of given channel.</summary>
        public IEnumerable<LocalVoice> LocalVoicesInChannel(int channelId)
        {
            List<LocalVoice> channelVoices;
            if (this.localVoicesPerChannel.TryGetValue(channelId, out channelVoices))
            {
                var res = new LocalVoice[channelVoices.Count];
                channelVoices.CopyTo(res, 0);
                return res;
            }
            else
            {
                return new LocalVoice[0];
            }
        }
        /// <summary>Iterates through all remote voices infos.</summary>
        public IEnumerable<RemoteVoiceInfo> RemoteVoiceInfos
        {
            get
            {
                foreach (var playerVoices in this.remoteVoices)
                {
                    foreach (var voice in playerVoices.Value)
                    {
                        yield return new RemoteVoiceInfo(voice.Value.channelId, playerVoices.Key, voice.Key, voice.Value.Info);
                    }
                }
            }
        }
        /// <summary>Creates VoiceClient instance</summary>
        internal VoiceClient(IVoiceTransport transport)
        {
            this.transport = transport;
        }
        /// <summary>
        /// This method dispatches all available incoming commands and then sends this client's outgoing commands.
        /// Call this method regularly (2..20 times a second).
        /// </summary>
        public void Service()
        {
            foreach (var v in localVoices)
            {
                v.Value.service();
            }
        }
        private LocalVoice createLocalVoice(int channelId, Func<byte, int, LocalVoice> voiceFactory)
        {
            var newId = getNewVoiceId();
            if (newId != 0)
            {
                LocalVoice v = voiceFactory(newId, channelId);
                if (v != null)
                {
                    addVoice(newId, channelId, v);
                    this.transport.LogInfo(v.LogPrefix + " added enc: " + v.info.ToString());
                    return v;
                }
            }
            return null;
        }
        /// <summary>
        /// Creates basic outgoing stream w/o data processing support. Provided encoder should generate output data stream.
        /// </summary>
        /// <param name="voiceInfo">Outgoing stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="channelId">Transport channel specific to transport.</param>
        /// <param name="encoder">Encoder producing the stream.</param>
        /// <returns>Outgoing stream handler.</returns>
        public LocalVoice CreateLocalVoice(VoiceInfo voiceInfo, int channelId = 0, IEncoder encoder = null)
        {
            return (LocalVoice)createLocalVoice(channelId, (vId, chId) => new LocalVoice(this, encoder, vId, voiceInfo, chId));
        }
        /// <summary>
        /// Creates outgoing stream consuming sequence of values passed in array buffers of arbitrary length which repacked in frames of constant length for further processing and encoding.
        /// </summary>
        /// <typeparam name="T">Type of data consumed by outgoing stream (element type of array buffers).</typeparam>
        /// <param name="voiceInfo">Outgoing stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="frameSize">Size of buffer LocalVoiceFramed repacks input data stream to.</param>
        /// <param name="channelId">Transport channel specific to transport.</param>
        /// <param name="encoder">Encoder compressing data stream in pipeline.</param>
        /// <returns>Outgoing stream handler.</returns>
        public LocalVoiceFramed<T> CreateLocalVoiceFramed<T>(VoiceInfo voiceInfo, int frameSize, int channelId = 0, IEncoder encoder = null)
        {
            return (LocalVoiceFramed<T>)createLocalVoice(channelId, (vId, chId) => new LocalVoiceFramed<T>(this, encoder, vId, voiceInfo, chId, frameSize));
        }
        /// <summary>
        /// Creates outgoing audio stream. Adds audio specific features (e.g. resampling, level meter) to processing pipeline and to returning stream handler.
        /// </summary>
        /// <typeparam name="T">Element type of audio array buffers.</typeparam>
        /// <param name="voiceInfo">Outgoing audio stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="channelId">Transport channel specific to transport.</param>
        /// <param name="encoder">Audio encoder. Set to null to use default Opus encoder.</param>
        /// <returns>Outgoing stream handler.</returns>
        /// <remarks>
        /// audioSourceDesc.SamplingRate and voiceInfo.SamplingRate may do not match. Automatic resampling will occur in this case.
        /// </remarks>
        public LocalVoiceAudio<T> CreateLocalVoiceAudio<T>(VoiceInfo voiceInfo, IAudioDesc audioSourceDesc, int channelId = 0, IEncoder encoder = null)
        {
            return (LocalVoiceAudio<T>)createLocalVoice(channelId, (vId, chId) => LocalVoiceAudio<T>.Create(this, vId, encoder, voiceInfo, audioSourceDesc, chId));
        }
        /// <summary>
        /// Creates outgoing audio stream of type automatically assigned and adds procedures (callback or serviceable) for consuming given audio source data.
        /// Adds audio specific features (e.g. resampling, level meter) to processing pipeline and to returning stream handler.
        /// </summary>
        /// <param name="voiceInfo">Outgoing audio stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="source">Streaming audio source.</param>
        /// <param name="forceShort">For audio sources producing buffers of 'float' type, creates stream of 'short' type and adds converter.</param>
        /// <param name="channelId">Transport channel specific to transport.</param>
        /// <param name="encoder">Audio encoder. Set to null to use default Opus encoder.</param>
        /// <returns>Outgoing stream handler.</returns>
        /// <remarks>
        /// audioSourceDesc.SamplingRate and voiceInfo.SamplingRate may do not match. Automatic resampling will occur in this case.
        /// </remarks>
        public LocalVoice CreateLocalVoiceAudioFromSource(VoiceInfo voiceInfo, IAudioDesc source, bool forceShort = false, int channelId = 0, IEncoder encoder = null)
        {
            if (source is IAudioPusher<float>)
            {
                if (forceShort)
                {
                    var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, source, channelId, encoder);
                    // we can safely reuse the same buffer in callbacks from native code
                    // 
                    var bufferFactory = new FactoryReusableArray<float>(0);
                    ((IAudioPusher<float>)source).SetCallback(buf => {
                        var shortBuf = localVoice.BufferFactory.New(buf.Length);
                        AudioUtil.Convert(buf, shortBuf, buf.Length);
                        localVoice.PushDataAsync(shortBuf);
                    }, bufferFactory);
                    return localVoice;
                }
                else
                {
                    var localVoice = CreateLocalVoiceAudio<float>(voiceInfo, source, channelId, encoder);
                    ((IAudioPusher<float>)source).SetCallback(buf => localVoice.PushDataAsync(buf), localVoice.BufferFactory);
                    return localVoice;
                }
            }
            else if (source is IAudioPusher<short>)
            {
                var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, source, channelId, encoder);
                ((IAudioPusher<short>)source).SetCallback(buf => localVoice.PushDataAsync(buf), localVoice.BufferFactory);
                return localVoice;
            }
            else if (source is IAudioReader<float>)
            {
                if (forceShort)
                {
                    transport.LogInfo("[PV] Creating local voice with source samples type conversion from float to short.");
                    var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, source, channelId, encoder);
                    localVoice.LocalUserServiceable = new BufferReaderPushAdapterAsyncPoolFloatToShort(localVoice, source as IAudioReader<float>);
                    return localVoice;
                }
                else
                {
                    var localVoice = CreateLocalVoiceAudio<float>(voiceInfo, source, channelId, encoder);
                    localVoice.LocalUserServiceable = new BufferReaderPushAdapterAsyncPool<float>(localVoice, source as IAudioReader<float>);
                    return localVoice;
                }
            }
            else if (source is IAudioReader<short>)
            {
                var localVoice = CreateLocalVoiceAudio<short>(voiceInfo, source, channelId, encoder);
                localVoice.LocalUserServiceable = new BufferReaderPushAdapterAsyncPool<short>(localVoice, source as IAudioReader<short>);
                return localVoice;
            }
            else
            {
                transport.LogError("[PV] CreateLocalVoiceAudioFromSource does not support Voice.IAudioDesc of type {0}", source.GetType());
                return LocalVoiceAudioDummy.Dummy;
            }
        }
#if PHOTON_VOICE_VIDEO_ENABLE
        /// <summary>
        /// Creates outgoing video stream consuming sequence of image buffers.
        /// </summary>
        /// <param name="voiceInfo">Outgoing stream parameters. Set applicable fields to read them by encoder and by receiving client when voice created.</param>
        /// <param name="channelId">Transport channel specific to transport.</param>
        /// <param name="encoder">Encoder compressing video data. Set to null to use default VP8 implementation.</param>
        /// <returns>Outgoing stream handler.</returns>
        public LocalVoiceVideo CreateLocalVoiceVideo(VoiceInfo voiceInfo, int channelId = 0, IEncoder encoder = null)
        {
            return (LocalVoiceVideo)createLocalVoice(channelId, (vId, chId) => new LocalVoiceVideo(this, encoder, vId, voiceInfo, chId));
        }
#endif
        private byte getNewVoiceId()
        {
            // id assigned starting from 1 and up to 255
            byte newId = 0; // non-zero if successfully assigned
            if (voiceIdCnt == 255)
            {
                // try to reuse id
                var ids = new bool[256];
                foreach (var v in localVoices)
                {
                    ids[v.Value.id] = true;
                }
                // ids[0] is not used
                for (byte id = 1; id != 0 /* < 256 */ ; id++)
                {
                    if (!ids[id])
                    {
                        newId = id;
                        break;
                    }
                }
            }
            else
            {
                voiceIdCnt++;
                newId = voiceIdCnt;
            }
            return newId;
        }
        void addVoice(byte newId, int channelId, LocalVoice v)
        {
            localVoices[newId] = v;
            List<LocalVoice> voiceList;
            if (!localVoicesPerChannel.TryGetValue(channelId, out voiceList))
            {
                voiceList = new List<LocalVoice>();
                localVoicesPerChannel[channelId] = voiceList;
            }
            voiceList.Add(v);
            if (this.transport.IsChannelJoined(channelId))
            {
                this.transport.SendVoicesInfo(new List<LocalVoice>() { v }, channelId, 0); // broadcast if joined
            }
            v.InterestGroup = this.GlobalInterestGroup;
        }
        /// <summary>
        /// Removes local voice (outgoing data stream).
        /// <param name="voice">Handler of outgoing stream to be removed.</param>
        /// </summary>
        public void RemoveLocalVoice(LocalVoice voice)
        {
            this.localVoices.Remove(voice.id);
            this.localVoicesPerChannel[voice.channelId].Remove(voice);
            if (this.transport.IsChannelJoined(voice.channelId))
            {
                this.transport.SendVoiceRemove(voice, voice.channelId, 0);
            }
            voice.Dispose();
            this.transport.LogInfo(voice.LogPrefix + " removed");
        }
        private void sendVoicesInfo(int channelId, int targetPlayerId)
        {
            sendChannelVoicesInfo(channelId, targetPlayerId);
        }
        private void sendChannelVoicesInfo(int channelId, int targetPlayerId)
        {
            if (this.transport.IsChannelJoined(channelId))
            {
                List<LocalVoice> voiceList;
                if (this.localVoicesPerChannel.TryGetValue(channelId, out voiceList))
                {
                    this.transport.SendVoicesInfo(voiceList, channelId, targetPlayerId);
                }
            }
        }
        internal byte GlobalInterestGroup
        {
            get { return this.globalInterestGroup; }
            set
            {
                this.globalInterestGroup = value;
                foreach (var v in this.localVoices)
                {
                    v.Value.InterestGroup = this.globalInterestGroup;
                }
            }
        }
        #region nonpublic
        private byte globalInterestGroup;
        private byte voiceIdCnt = 0;
        private Dictionary<byte, LocalVoice> localVoices = new Dictionary<byte, LocalVoice>();
        private Dictionary<int, List<LocalVoice>> localVoicesPerChannel = new Dictionary<int, List<LocalVoice>>();
        // player id -> voice id -> voice
        private Dictionary<int, Dictionary<byte, RemoteVoice>> remoteVoices = new Dictionary<int, Dictionary<byte, RemoteVoice>>();
        private void clearRemoteVoices()
        {
            foreach (var playerVoices in remoteVoices)
            {
                foreach (var voice in playerVoices.Value)
                {
                    voice.Value.removeAndDispose();
                }
            }
            remoteVoices.Clear();
            this.transport.LogInfo("[PV] Remote voices cleared");
        }
        private void clearRemoteVoicesInChannel(int channelId)
        {
            foreach (var playerVoices in remoteVoices)
            {
                List<byte> toRemove = new List<byte>();
                foreach (var voice in playerVoices.Value)
                {
                    if (voice.Value.channelId == channelId)
                    {
                        voice.Value.removeAndDispose();
                        toRemove.Add(voice.Key);
                    }
                }
                foreach (var id in toRemove)
                {
                    playerVoices.Value.Remove(id);
                }
            }
            this.transport.LogInfo("[PV] Remote voices for channel " + this.channelStr(channelId) + " cleared");
        }
        private void clearRemoteVoicesInChannelForPlayer(int channelId, int playerId)
        {
            Dictionary<byte, RemoteVoice> playerVoices = null;
            if (remoteVoices.TryGetValue(playerId, out playerVoices))
            {
                List<byte> toRemove = new List<byte>();
                foreach (var v in playerVoices)
                {
                    if (v.Value.channelId == channelId)
                    {
                        v.Value.removeAndDispose();
                        toRemove.Add(v.Key);
                    }
                }
                foreach (var id in toRemove)
                {
                    playerVoices.Remove(id);
                }
            }
        }
        
		internal void onJoinChannel(int channel)
        {
            sendChannelVoicesInfo(channel, 0);// my join, broadcast
        }
        internal void onLeaveChannel(int channel)
        {
            clearRemoteVoicesInChannel(channel);
        }
        internal void onLeaveAllChannels()
        {
            clearRemoteVoices();
        }
        internal void onPlayerJoin(int channelId, int playerId)
        {
            sendVoicesInfo(channelId, playerId);// send to new joined only
        }
        internal void onPlayerLeave(int channelId, int playerId)
        {
            clearRemoteVoicesInChannelForPlayer(channelId, playerId);
        }
        internal void onVoiceInfo(int channelId, int playerId, byte voiceId, byte eventNumber, VoiceInfo info)
        {
            Dictionary<byte, RemoteVoice> playerVoices = null;
            if (!remoteVoices.TryGetValue(playerId, out playerVoices))
            {
                playerVoices = new Dictionary<byte, RemoteVoice>();
                remoteVoices[playerId] = playerVoices;
            }
            if (!playerVoices.ContainsKey(voiceId))
            {
                this.transport.LogInfo("[PV] ch#" + this.channelStr(channelId) + " p#" + this.playerStr(playerId) + " v#" + voiceId + " Info received: " + info.ToString() + " ev=" + eventNumber);
                RemoteVoiceOptions options = new RemoteVoiceOptions() { OutputImageFormat = ImageFormat.Undefined, OutputImageFlip = Flip.Undefined };
                if (this.OnRemoteVoiceInfoAction != null)
                {
                    this.OnRemoteVoiceInfoAction(channelId, playerId, voiceId, info, ref options);
                }
                playerVoices[voiceId] = new RemoteVoice(this, options, channelId, playerId, voiceId, info, eventNumber);
            }
            else
            {
                if (!this.SuppressInfoDuplicateWarning)
                {
                    this.transport.LogWarning("[PV] Info duplicate for voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId));
                }
            }
        }
        internal void onVoiceRemove(int channelId, int playerId, byte[] voiceIds)
        {
            Dictionary<byte, RemoteVoice> playerVoices = null;
            if (remoteVoices.TryGetValue(playerId, out playerVoices))
            {
                foreach (var voiceId in voiceIds)
                {
                    RemoteVoice voice;
                    if (playerVoices.TryGetValue(voiceId, out voice))
                    {
                        playerVoices.Remove(voiceId);
                        this.transport.LogInfo("[PV] Remote voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId) + " removed");
                        voice.removeAndDispose();
                    }
                    else
                    {
                        this.transport.LogWarning("[PV] Remote voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId) + " not found when trying to remove");
                    }
                }
            }
            else
            {
                this.transport.LogWarning("[PV] Remote voice list of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId) + " not found when trying to remove voice(s)");
            }
        }
        Random rnd = new Random();
        internal void onFrame(int channelId, int playerId, byte voiceId, byte evNumber, byte[] receivedBytes, bool isLocalPlayer)
        {
            if (isLocalPlayer)
            {
                // rtt measurement in debug echo mode
                LocalVoice voice;
                if (this.localVoices.TryGetValue(voiceId, out voice))
                {
                    int sendTime;
                    if (voice.eventTimestamps.TryGetValue(evNumber, out sendTime))
                    {
                        int rtt = Environment.TickCount - sendTime;
                        int rttvar = rtt - prevRtt;
                        prevRtt = rtt;
                        if (rttvar < 0) rttvar = -rttvar;
                        this.RoundTripTimeVariance = (rttvar + RoundTripTimeVariance * 19) / 20;
                        this.RoundTripTime = (rtt + RoundTripTime * 19) / 20;
                    }
                }
                //internal Dictionary<byte, DateTime> localEventTimestamps = new Dictionary<byte, DateTime>();
            }
            if (this.DebugLostPercent > 0 && rnd.Next(100) < this.DebugLostPercent)
            {
                this.transport.LogWarning("[PV] Debug Lost Sim: 1 packet dropped");
                return;
            }
            FramesReceived++;
            Dictionary<byte, RemoteVoice> playerVoices = null;
            if (remoteVoices.TryGetValue(playerId, out playerVoices))
            {
                RemoteVoice voice = null;
                if (playerVoices.TryGetValue(voiceId, out voice))
                {
                    voice.receiveBytes(receivedBytes, evNumber);
                }
                else
                {
                    this.transport.LogWarning("[PV] Frame event for not inited voice #" + voiceId + " of player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId));
                }
            }
            else
            {
                this.transport.LogWarning("[PV] Frame event for voice #" + voiceId + " of not inited player " + this.playerStr(playerId) + " at channel " + this.channelStr(channelId));
            }
        }
        internal string channelStr(int channelId)
        {
            var str = this.transport.ChannelIdStr(channelId);
            if (str != null)
            {
                return channelId + "(" + str + ")";
            }
            else
            {
                return channelId.ToString();
            }
        }
        internal string playerStr(int playerId)
        {
            var str = this.transport.PlayerIdStr(playerId);
            if (str != null)
            {
                return playerId + "(" + str + ")";
            }
            else
            {
                return playerId.ToString();
            }
        }
        //public string ToStringFull()
        //{
        //    return string.Format("Photon.Voice.Client, local: {0}, remote: {1}",  localVoices.Count, remoteVoices.Count);
        //}
        #endregion
        public void Dispose()
        {
            foreach (var v in this.localVoices)
            {
                v.Value.Dispose();
            }
            foreach (var playerVoices in remoteVoices)
            {
                foreach (var voice in playerVoices.Value)
                {
                    voice.Value.Dispose();
                }
            }
        }
    }
}
