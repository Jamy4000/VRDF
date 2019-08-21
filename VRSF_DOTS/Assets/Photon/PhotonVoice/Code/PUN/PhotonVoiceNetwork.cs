// ----------------------------------------------------------------------------
// <copyright file="PhotonVoiceNetwork.cs" company="Exit Games GmbH">
// Photon Voice - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// This class can be used to automatically join/leave Voice rooms when
// Photon Unity Networking (PUN) joins or leaves its rooms. The Voice room
// will use the same name as PUN, but with a "_voice_" postfix.
// It also sets a custom PUN Speaker factory to find the Speaker
// component for a character's voice. For this to work, the voice's UserData
// must be set to the character's PhotonView ID. 
// (see "PhotonVoiceView.cs")
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity;

namespace Photon.Voice.PUN
{

    /// <summary>
    /// This class can be used to automatically sync client states between PUN and Voice.
    /// It also sets a custom PUN Speaker factory to find the Speaker component for a character's voice. 
    /// For this to work attach a <see cref="PhotonVoiceView"/> next to the <see cref="PhotonView"/> of your player's prefab.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Photon Voice/Photon Voice Network")]
    [HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-for-pun")]
    public class PhotonVoiceNetwork : VoiceConnection
    {
        #region Public Fields

        /// <summary> Suffix for voice room names appended to PUN room names. </summary>
        public const string VoiceRoomNameSuffix = "_voice_";
        /// <summary> Auto connect voice client and join a voice room when PUN client is joined to a PUN room </summary>
        public bool AutoConnectAndJoin = true;
        /// <summary> Auto disconnect voice client when PUN client is not joined to a PUN room </summary>
        public bool AutoLeaveAndDisconnect = true;
        /// <summary> Auto instantiate a GameObject and attach a Speaker component to link to a remote audio stream if no candidate could be found </summary>
        public bool AutoCreateSpeakerIfNotFound = true;

        #endregion

        #region Private Fields

        private RoomOptions voiceRoomOptions = new RoomOptions { IsVisible = false };
        private bool clientCalledConnectAndJoin;
        private bool clientCalledDisconnect;
        private static object instanceLock = new object();
        private static PhotonVoiceNetwork instance;
        private static bool instantiated;

        [SerializeField]
        private bool usePunAppSettings = true;

        #endregion

        #region Properties

        /// <summary>
        /// Singleton instance for PhotonVoiceNetwork
        /// </summary>
        public static PhotonVoiceNetwork Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (AppQuits)
                    {
                        if (instance.Logger.IsWarningEnabled)
                        {
                            instance.Logger.LogWarning("PhotonVoiceNetwork Instance already destroyed on application quit. Won't create again - returning null.");
                        }
                        return null;
                    }
                    if (!instantiated)
                    {
                        PhotonVoiceNetwork[] objects = FindObjectsOfType<PhotonVoiceNetwork>();
                        if (objects == null || objects.Length < 1)
                        {
                            GameObject singleton = new GameObject();
                            singleton.name = "PhotonVoiceNetwork singleton";
                            instance = singleton.AddComponent<PhotonVoiceNetwork>();
                            if (instance.Logger.IsWarningEnabled)
                            {
                                instance.Logger.LogWarning("An instance of PhotonVoiceNetwork was automatically created in the scene.");
                            }
                        }
                        else if (objects.Length >= 1)
                        {
                            instance = objects[0];
                            if (objects.Length > 1)
                            {
                                if (instance.Logger.IsWarningEnabled)
                                {
                                    instance.Logger.LogWarning("{0} PhotonVoiceNetwork instances found. Using first one only.", objects.Length);
                                }
                            }
                        }
                        instantiated = true;
                    }
                    return instance;
                }
            }
            set
            {
                lock (instanceLock)
                {
                    if (value == null)
                    {
                        if (instance.Logger.IsWarningEnabled)
                        {
                            instance.Logger.LogWarning("Cannot set PhotonVoiceNetwork.Instance to null.");
                        }
                        return;
                    }
                    if (instantiated)
                    {
                        if (instance.GetInstanceID() != value.GetInstanceID())
                        {
                            if (instance.Logger.IsWarningEnabled)
                            {
                                instance.Logger.LogWarning("An instance of PhotonVoiceNetwork is already set.");
                            }
                        }
                        return;
                    }
                    instantiated = true;
                    instance = value;
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect voice client to Photon servers and join a Voice room
        /// </summary>
        /// <returns>If true, connection command send from client</returns>
        public bool ConnectAndJoinRoom()
        {
            if (!PhotonNetwork.InRoom)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Cannot connect and join if PUN is not joined.");
                }
                return false;
            }
            AppSettings settings = this.Settings;
            if (this.usePunAppSettings)
            {
                settings = PhotonNetwork.PhotonServerSettings.AppSettings;
            }
            if (PhotonNetwork.NetworkingClient != null && !string.IsNullOrEmpty(PhotonNetwork.NetworkingClient.UserId))
            {
                this.Client.UserId = PhotonNetwork.NetworkingClient.UserId;
            }
            if (this.ConnectUsingSettings(settings))
            {
                clientCalledConnectAndJoin = true;
                clientCalledDisconnect = false;
                return true;
            }
            if (this.Logger.IsErrorEnabled)
            {
                this.Logger.LogError("Connecting to server failed.");
            }
            return false;
        }

        /// <summary>
        /// Disconnect voice client from all Photon servers
        /// </summary>
        public void Disconnect()
        {
            if (!this.Client.IsConnected)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Cannot Disconnect if not connected.");
                }
                return;
            }
            clientCalledDisconnect = true;
            clientCalledConnectAndJoin = false;
            this.Client.Disconnect();
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        private void OnEnable()
        {
            PhotonNetwork.NetworkingClient.StateChanged += OnPunStateChanged;
            FollowPun(); // in case this is enabled or activated late
            clientCalledConnectAndJoin = false;
            clientCalledDisconnect = false;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PhotonNetwork.NetworkingClient.StateChanged -= OnPunStateChanged;
        }

        protected override void OnDestroy()
        {
            lock (instanceLock)
            {
                instantiated = false;
            }
        }

        private void OnPunStateChanged(ClientState fromState, ClientState toState)
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnPunStateChanged from {0} to {1}", fromState, toState);
            }
            this.FollowPun(toState);
        }

        protected override void OnVoiceStateChanged(ClientState fromState, ClientState toState)
        {
            base.OnVoiceStateChanged(fromState, toState);
            if (!this.clientCalledDisconnect && toState == ClientState.Disconnected && this.Client.DisconnectedCause == DisconnectCause.DisconnectByClientLogic)
            {
                this.clientCalledDisconnect = true;
            }
            this.FollowPun(toState);
        }

        private void FollowPun(ClientState toState)
        {
            switch (toState)
            {
                case ClientState.Joined:
                case ClientState.Disconnected:
                case ClientState.ConnectedToMasterServer:
                    this.FollowPun();
                    break;
            }
        }

        protected override Speaker SimpleSpeakerFactory(int playerId, byte voiceId, object userData)
        {
            if (!(userData is int))
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("UserData ({0}) does not contain PhotonViewId. Remote voice {1}/{2} not linked",
                        userData == null ? "null" : userData.ToString(), playerId, voiceId);
                }
                return null;
            }

            int photonViewId = (int)userData;
            PhotonView photonView = PhotonView.Find(photonViewId);
            if (photonView == null)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("No PhotonView with ID {0} found. Remote voice {1}/{2} not linked.", userData, playerId, voiceId);
                }
                return null;
            }

            PhotonVoiceView photonVoiceView = photonView.GetComponent<PhotonVoiceView>();
            if (photonVoiceView == null)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("No PhotonVoiceView attached to the PhotonView with ID {0}. Remote voice {1}/{2} not linked.", userData, playerId, voiceId);
                }
                return null;
            }
            if (!photonVoiceView.IsSpeaker)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("No Speaker found for the PhotonView with ID {0}. Remote voice {1}/{2} not linked.", userData, playerId, voiceId);
                }
                return null;
            }
            return photonVoiceView.SpeakerInUse;
        }

        internal static string GetVoiceRoomName()
        {
            if (PhotonNetwork.InRoom)
            {
                return string.Format("{0}{1}", PhotonNetwork.CurrentRoom.Name, VoiceRoomNameSuffix);
            }
            return null;
        }

        private void ConnectOrJoin()
        {
            switch (this.ClientState)
            {
                case ClientState.PeerCreated:
                case ClientState.Disconnected:
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("PUN joined room, now connecting Voice client");
                    }
                    if (!this.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings))
                    {
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("Connecting to server failed.");
                        }
                    }
                    break;
                case ClientState.ConnectedToMasterServer:
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("PUN joined room, now joining Voice room");
                    }
                    JoinRoom(GetVoiceRoomName());
                    break;
                default:
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("PUN joined room, Voice client is busy ({0}). Is this expected?", this.ClientState);
                    }
                    break;
            }
        }

        private void JoinRoom(string voiceRoomName)
        {
            if (string.IsNullOrEmpty(voiceRoomName))
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Voice room name is null or empty.");
                }
                return;
            }
            this.Client.OpJoinOrCreateRoom(new EnterRoomParams { RoomName = voiceRoomName, RoomOptions = voiceRoomOptions });
        }

        // Follow PUN client state
        // In case Voice client disconnects unexpectedly try to reconnect to the same room
        // In case Voice client is connected to the wrong room switch to the correct one
        private void FollowPun()
        {
            if (AppQuits)
            {
                return;
            }
            if (PhotonNetwork.NetworkClientState == this.ClientState)
            {
                if (PhotonNetwork.NetworkClientState == ClientState.Joined && this.AutoConnectAndJoin)
                {
                    string expectedRoomName = GetVoiceRoomName();
                    string currentRoomName = this.Client.CurrentRoom.Name;
                    if (!currentRoomName.Equals(expectedRoomName))
                    {
                        if (this.Logger.IsWarningEnabled)
                        {
                            this.Logger.LogWarning(
                                "Voice room mismatch: Expected:\"{0}\" Current:\"{1}\", leaving the second to join the first.",
                                expectedRoomName, currentRoomName);
                        }
                        this.Client.OpLeaveRoom(false);
                    }
                }
                return;
            }
            if (PhotonNetwork.InRoom)
            {
                if (clientCalledConnectAndJoin || AutoConnectAndJoin && !clientCalledDisconnect)
                {
                    ConnectOrJoin();
                }
            }
            else if (this.ClientState == ClientState.Joined && AutoLeaveAndDisconnect && !clientCalledConnectAndJoin)
            {
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("PUN left room, disconnecting Voice");
                }
                this.Client.Disconnect();
            }
        }

        internal void CheckLateLinking(PhotonVoiceView photonVoiceView, int viewId)
        {
            if (this.Client.InRoom && photonVoiceView != null && viewId > 0)
            {
                for (int i = 0; i < cachedRemoteVoices.Count; i++)
                {
                    RemoteVoiceLink remoteVoice = cachedRemoteVoices[i];
                    if (remoteVoice.Info.UserData is int)
                    {
                        int photonViewId = (int)remoteVoice.Info.UserData;
                        if (viewId == photonViewId)
                        {
                            Speaker speaker = photonVoiceView.SpeakerInUse;
                            if (this.Logger.IsInfoEnabled)
                            {
                                this.Logger.LogInfo("Speaker 'late-linking' for the PhotonView with ID {0} with remote voice {1}/{2}.", viewId, remoteVoice.PlayerId, remoteVoice.VoiceId);
                            }
                            LinkSpeaker(speaker, remoteVoice);
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
