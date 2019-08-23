// ----------------------------------------------------------------------------
// <copyright file="Recorder.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//  Component that represents a client voice connection to Photon Servers.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
#if UNITY_5_5_OR_NEWER
using UnityEngine.Profiling;
#endif

namespace Photon.Voice.Unity
{
    /// <summary> Component that represents a client voice connection to Photon Servers. </summary>
    [AddComponentMenu("Photon Voice/Voice Connection")]
    [HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-intro")]
    public class VoiceConnection : ConnectionHandler, ILoggable
    {
        #region Private Fields

        private VoiceLogger logger;

        [SerializeField]
        private DebugLevel logLevel = DebugLevel.ERROR;

        /// <summary>Key to save the "Best Region Summary" in the Player Preferences.</summary>
        private const string PlayerPrefsKey = "VoiceCloudBestRegion";
        
        private LoadBalancingTransport client;
        [SerializeField]
        private bool enableSupportLogger;

        private SupportLogger supportLoggerComponent;

        /// <summary>
        /// time [ms] between consecutive SendOutgoingCommands calls
        /// </summary>
        [SerializeField]
        private int updateInterval = 50;

        private int nextSendTickCount;

        // Used in the main thread, OnRegionsPinged is called in a separate thread and so we can't use some of the Unity methods (like saying in playerPrefs)
        private RegionHandler cachedRegionHandler;

        #if !UNITY_ANDROID && !UNITY_IOS
        [SerializeField]
        private bool runInBackground = true;
        #endif

        /// <summary>
        /// time [ms] between statistics calculations
        /// </summary>
        [SerializeField]
        private int statsResetInterval = 1000;

        private int nextStatsTickCount;

        private float statsReferenceTime;
        private int referenceFramesLost;
        private int referenceFramesReceived;

        [SerializeField]
        private GameObject speakerPrefab;

        private bool cleanedUp;

        protected List<RemoteVoiceLink> cachedRemoteVoices = new List<RemoteVoiceLink>();

        #endregion

        #region Public Fields

        /// <summary> Settings to be used by this voice connection</summary>
        public AppSettings Settings;
        #if UNITY_EDITOR
        [HideInInspector]
        public bool ShowSettings;
        #endif
        /// <summary> Main Recorder to be used for transmission by default</summary>
        public Recorder PrimaryRecorder;

        /// <summary> Special factory to link Speaker components with incoming remote audio streams</summary>
        public Func<int, byte, object, Speaker> SpeakerFactory;
        /// <summary> Fires when a speaker has been linked to a remote audio stream</summary>
        public event Action<Speaker> SpeakerLinked;
        /// <summary> Fires when a remote voice stream is added</summary>
        public event Action<RemoteVoiceLink> RemoteVoiceAdded;
        
        #if UNITY_PS4
        /// <summary>PS4 user ID of the local user</summary>
        /// <remarks>Pass the userID of the PS4 controller that is used by the local user.This value is used by Photon Voice when sending output to the headphones of as PS4 controller.
        /// If you don't provide a user ID, then Photon Voice uses the user ID of the user at index 0 in the list of local users
        /// and in case that multiple controllers are attached, the audio output might be sent to the headphones of a different controller then intended.</remarks>
        public int PS4UserID = 0;                       // set from your games code
        #endif
        
        #endregion

        #region Properties
        /// <summary> Logger used by this component</summary>
        public VoiceLogger Logger
        {
            get
            {
                if (logger == null)
                {
                    logger = new VoiceLogger(this, string.Format("{0}.{1}", name, this.GetType().Name), logLevel);
                }
                return logger;
            }
            protected set { logger = value; }
        }
        /// <summary> Log level for this component</summary>
        public DebugLevel LogLevel
        {
            get
            {
                if (this.Logger != null)
                {
                    logLevel = this.Logger.LogLevel;
                }
                return logLevel;
            }
            set
            {
                logLevel = value;
                if (this.Logger == null)
                {
                    return;
                }
                this.Logger.LogLevel = logLevel;
            }
        }

        public new LoadBalancingTransport Client
        {
            get
            {
                if (client == null)
                {
                    client = new LoadBalancingTransport();
                    client.VoiceClient.OnRemoteVoiceInfoAction += OnRemoteVoiceInfo;
                    client.OpResponseReceived += OnOperationResponse;
                    client.StateChanged += OnVoiceStateChanged;
                    base.Client = client;
                    this.StartFallbackSendAckThread();
                }
                return client;
            }
        }
        
        /// <summary>Returns underlying Photon Voice client.</summary>
        public VoiceClient VoiceClient { get { return this.Client.VoiceClient; } }

        /// <summary>Returns Photon Voice client state.</summary>
        public ClientState ClientState { get { return this.Client.State; } }

        /// <summary>Number of frames received per second.</summary>
        public float FramesReceivedPerSecond { get; private set; }
        /// <summary>Number of frames lost per second.</summary>
        public float FramesLostPerSecond { get; private set; }
        /// <summary>Percentage of lost frames.</summary>
        public float FramesLostPercent { get; private set; }

        /// <summary> Prefab that contains Speaker component to be instantiated when receiving a new remote audio source info</summary>
        public GameObject SpeakerPrefab
        {
            get { return this.speakerPrefab; }
            set
            {
                if (value != this.speakerPrefab)
                {
                    if (value != null && value.GetComponentInChildren<Speaker>() == null)
                    {
                        #if UNITY_EDITOR
                        Debug.LogError("SpeakerPrefab must have a component of type Speaker in its hierarchy.", this);
                        #else
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("SpeakerPrefab must have a component of type Speaker in its hierarchy.");
                        }
                        #endif
                        return;
                    }
                    this.speakerPrefab = value;
                }
            }
        }

        
        #if UNITY_EDITOR
        public List<RemoteVoiceLink> CachedRemoteVoices
        {
            get { return this.cachedRemoteVoices; }
        }
        #endif

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect to Photon server using <see cref="Settings"/>
        /// </summary>
        /// <param name="overwriteSettings">Overwrites <see cref="Settings"/> before connecting</param>
        /// <returns>If true voice connection command was sent from client</returns>
        public bool ConnectUsingSettings(AppSettings overwriteSettings = null)
        {
            if (this.Client.LoadBalancingPeer.PeerState != PeerStateValue.Disconnected)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("ConnectUsingSettings() failed. Can only connect while in state 'Disconnected'. Current state: {0}", this.Client.LoadBalancingPeer.PeerState);
                }
                return false;
            }
            if (overwriteSettings != null)
            {
                Settings = overwriteSettings;
            }
            if (Settings == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Settings are null");
                }
                return false;
            }
            if (string.IsNullOrEmpty(Settings.AppIdVoice) && string.IsNullOrEmpty(Settings.Server))
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Provide an AppId or a Server address in Settings to be able to connect");
                }
                return false;
            }

            if (Settings.Protocol == ConnectionProtocol.Tcp)
            {
                if (!Settings.IsMasterServerAddress)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Requested protocol not supported on Photon Cloud {0}. Switched to UDP.", Settings.Protocol);
                    }
                    this.Client.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.Udp;
                }
                else
                {
                    this.Client.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.Tcp;
                }
            }
            else if (Settings.Protocol != ConnectionProtocol.Udp)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Requested protocol not supported: {0}. Switched to UDP.", Settings.Protocol);
                }
                this.Client.LoadBalancingPeer.TransportProtocol = ConnectionProtocol.Udp;
            }

            this.Client.EnableLobbyStatistics = Settings.EnableLobbyStatistics;

            this.Client.LoadBalancingPeer.DebugOut = Settings.NetworkLogging;

            if (Settings.IsMasterServerAddress)
            {
                this.Client.LoadBalancingPeer.SerializationProtocolType = SerializationProtocol.GpBinaryV16;

                if (string.IsNullOrEmpty(this.Client.UserId))
                {
                    this.Client.UserId = Guid.NewGuid().ToString();
                }

                this.Client.IsUsingNameServer = false;
                this.Client.MasterServerAddress = Settings.Port == 0 ? Settings.Server : string.Format("{0}:{1}", Settings.Server, Settings.Port);

                return this.Client.Connect();
            }

            this.Client.AppId = Settings.AppIdVoice;
            this.Client.AppVersion = Settings.AppVersion;

            if (!Settings.IsDefaultNameServer)
            {
                this.Client.NameServerHost = Settings.Server;
            }

            if (Settings.IsBestRegion)
            {
                return this.Client.ConnectToNameServer();
            }

            return this.Client.ConnectToRegionMaster(Settings.FixedRegion);
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            if (this.SpeakerFactory == null)
            {
                this.SpeakerFactory = SimpleSpeakerFactory;
            }
            if (enableSupportLogger)
            {
                this.supportLoggerComponent = this.gameObject.AddComponent<SupportLogger>();
                this.supportLoggerComponent.Client = this.Client;
                this.supportLoggerComponent.LogTrafficStats = true;
            }
            #if !UNITY_ANDROID && !UNITY_IOS
            if (runInBackground)
            {
                Application.runInBackground = runInBackground;
            }
            #endif
        }

        protected virtual void Update()
        {
            this.VoiceClient.Service();
        }

        protected virtual void FixedUpdate()
        {
            bool doDispatch = true;
            while (doDispatch)
            {
                // DispatchIncomingCommands() returns true of it found any command to dispatch (event, result or state change)
                Profiler.BeginSample("[Photon Voice]: DispatchIncomingCommands");
                doDispatch = this.Client.LoadBalancingPeer.DispatchIncomingCommands();
                Profiler.EndSample();
            }
        }

        private void LateUpdate()
        {
            int currentMsSinceStart = (int)(Time.realtimeSinceStartup * 1000); // avoiding Environment.TickCount, which could be negative on long-running platforms
            if (currentMsSinceStart > this.nextSendTickCount)
            {
                bool doSend = true;
                while (doSend)
                {
                    // Send all outgoing commands
                    Profiler.BeginSample("[Photon Voice]: SendOutgoingCommands");
                    doSend = this.Client.LoadBalancingPeer.SendOutgoingCommands();
                    Profiler.EndSample();
                }

                this.nextSendTickCount = currentMsSinceStart + this.updateInterval;
            }

            if (currentMsSinceStart > this.nextStatsTickCount)
            {
                if (this.statsResetInterval > 0)
                {
                    this.CalcStatistics();
                    this.nextStatsTickCount = currentMsSinceStart + this.statsResetInterval;
                }
            }
        }

        protected override void OnDisable()
        {
            if (AppQuits)
            {
                this.CleanUp();
                SupportClass.StopAllBackgroundCalls();
            }
        }

        protected virtual void OnDestroy()
        {
            this.CleanUp();
        }

        protected virtual Speaker SimpleSpeakerFactory(int playerId, byte voiceId, object userData)
        {
            Speaker speaker;
            if (SpeakerPrefab)
            {
                GameObject go = Instantiate(SpeakerPrefab);
                speaker = go.GetComponentInChildren<Speaker>();
                if (speaker == null)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("SpeakerPrefab does not have a component of type Speaker in its hierarchy.");
                    }
                    return null;
                }
            }
            else
            {
                speaker = new GameObject().AddComponent<Speaker>();
            }

            // within a room, users are identified via the Realtime.Player class. this has a nickname and enables us to use custom properties, too
            speaker.Actor = (this.Client.CurrentRoom != null) ? this.Client.CurrentRoom.GetPlayer(playerId) : null;
            speaker.name = speaker.Actor != null && !string.IsNullOrEmpty(speaker.Actor.NickName) ? speaker.Actor.NickName : String.Format("Speaker for Player {0} Voice #{1}", playerId, voiceId);
            speaker.OnRemoteVoiceRemoveAction += DeleteVoiceOnRemoteVoiceRemove;
            return speaker;
        }

        internal void DeleteVoiceOnRemoteVoiceRemove(Speaker speaker)
        {
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Remote voice removed, delete speaker");
            }
            Destroy(speaker.gameObject);
        }
        
        private void OnRemoteVoiceInfo(int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, ref RemoteVoiceOptions options)
        {
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("OnRemoteVoiceInfo channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
            }
            bool duplicate = false;
            for (int i = 0; i < cachedRemoteVoices.Count; i++)
            {
                RemoteVoiceLink remoteVoiceLink = cachedRemoteVoices[i];
                if (remoteVoiceLink.PlayerId == playerId && remoteVoiceLink.VoiceId == voiceId)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Duplicate remote voice info event channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                    }
                    duplicate = true;
                    cachedRemoteVoices.RemoveAt(i);
                    break;
                }
            }
            RemoteVoiceLink remoteVoice = new RemoteVoiceLink(voiceInfo, playerId, voiceId, channelId, ref options);
            cachedRemoteVoices.Add(remoteVoice);
            if (RemoteVoiceAdded != null)
            {
                RemoteVoiceAdded(remoteVoice);
            }
            remoteVoice.RemoteVoiceRemoved += delegate
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("RemoteVoiceRemoved channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                }
                if (!cachedRemoteVoices.Remove(remoteVoice) && this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cached remote voice info not removed for channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                }
            };
            if (SpeakerFactory != null)
            {
                Speaker speaker = SpeakerFactory(playerId, voiceId, voiceInfo.UserData);
                if (speaker != null && duplicate && speaker.IsLinked)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Overriding speaker link for channel {0} player {1} voice #{2} userData {3}", channelId, playerId, voiceId, voiceInfo.UserData);
                    }
                    speaker.OnRemoteVoiceRemove();
                }
                LinkSpeaker(speaker, remoteVoice);
            }
        }

        private void OnOperationResponse(OperationResponse opResponse)
        {
            switch (opResponse.OperationCode)
            {
                case OperationCode.GetRegions:
                    if (Settings != null && Settings.IsBestRegion && this.Client.RegionHandler != null)
                    {
                        this.Client.RegionHandler.PingMinimumOfRegions(OnRegionsPinged, BestRegionSummaryInPreferences);
                    }
                    break;
            }
        }

        protected virtual void OnVoiceStateChanged(ClientState fromState, ClientState toState)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnVoiceStateChanged from {0} to {1}", fromState, toState);
            }
            if (fromState == ClientState.Joined)
            {
                this.ClearRemoteVoicesCache();
            }
        }

        /// <summary>Used to store and access the "Best Region Summary" in the Player Preferences.</summary>
        internal string BestRegionSummaryInPreferences
        {
            get
            {
                if (cachedRegionHandler != null)
                {
                    BestRegionSummaryInPreferences = cachedRegionHandler.SummaryToCache;
                    return cachedRegionHandler.SummaryToCache;
                }
                return PlayerPrefs.GetString(PlayerPrefsKey, null);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    PlayerPrefs.DeleteKey(PlayerPrefsKey);
                }
                else
                {
                    PlayerPrefs.SetString(PlayerPrefsKey, value);
                }
            }
        }

        private void OnRegionsPinged(RegionHandler regionHandler)
        {
            cachedRegionHandler = regionHandler;
            this.Client.ConnectToRegionMaster(regionHandler.BestRegion.Code);
        }
        
        protected void CalcStatistics()
        {
            float now = Time.time;
            int recv = this.VoiceClient.FramesReceived - this.referenceFramesReceived;
            int lost = this.VoiceClient.FramesLost - this.referenceFramesLost;
            float t = now - statsReferenceTime;

            if (t > 0f)
            {
                if (recv + lost > 0)
                {
                    this.FramesReceivedPerSecond = recv / t;
                    this.FramesLostPerSecond = lost / t;
                    this.FramesLostPercent = 100f * lost / (recv + lost);
                }
                else
                {
                    this.FramesReceivedPerSecond = 0f;
                    this.FramesLostPerSecond = 0f;
                    this.FramesLostPercent = 0f;
                }
            }

            referenceFramesReceived = this.VoiceClient.FramesReceived;
            referenceFramesLost = this.VoiceClient.FramesLost;
            statsReferenceTime = now;
        }

        private void CleanUp()
        {
            bool clientStillExists = this.client != null;
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogInfo("Client exists? {0}, already cleaned up? {1}", clientStillExists, this.cleanedUp);
            }
            if (this.cleanedUp)
            {
                return;
            }
            this.StopFallbackSendAckThread();
            if (clientStillExists)
            {
                this.client.OpResponseReceived -= OnOperationResponse;
                this.client.StateChanged -= OnVoiceStateChanged;
                this.client.Disconnect();
                if (this.client.LoadBalancingPeer != null)
                {
                    this.client.LoadBalancingPeer.Disconnect();
                    this.client.LoadBalancingPeer.StopThread();
                }
                this.client.Dispose();
            }
            this.cleanedUp = true;
        }

        protected void LinkSpeaker(Speaker speaker, RemoteVoiceLink remoteVoice)
        {
            if (speaker != null)
            {
                if (speaker.IsLinked)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Speaker already linked. Remote voice {0}/{1} not linked.",
                            remoteVoice.PlayerId, remoteVoice.VoiceId);
                    }
                    return;
                }
                speaker.OnRemoteVoiceInfo(remoteVoice);
                if (speaker.Actor == null && this.Client.CurrentRoom != null)
                {
                    speaker.Actor = this.Client.CurrentRoom.GetPlayer(remoteVoice.PlayerId);
                }
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Speaker linked with remote voice {0}/{1}", remoteVoice.PlayerId, remoteVoice.VoiceId);
                }
                if (SpeakerLinked != null)
                {
                    SpeakerLinked(speaker);
                }
            }
            else if (this.Logger.IsWarningEnabled)
            {
                this.Logger.LogWarning("Speaker is null. Remote voice {0}/{1} not linked.", remoteVoice.PlayerId, remoteVoice.VoiceId);
            }
        }

        private void ClearRemoteVoicesCache()
        {
            if (cachedRemoteVoices.Count > 0)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("{0} cached remote voices info cleared", cachedRemoteVoices.Count);
                }
                cachedRemoteVoices.Clear();
            }
        }

        #endregion
    }
}

namespace Photon.Voice
{
    [Obsolete("Class renamed. Use LoadBalancingTransport instead.")]
    public class LoadBalancingFrontend : LoadBalancingTransport
    {
    }
}