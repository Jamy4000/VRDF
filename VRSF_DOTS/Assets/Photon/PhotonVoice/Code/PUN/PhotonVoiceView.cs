// ----------------------------------------------------------------------------
// <copyright file="PhotonVoiceView.cs" company="Exit Games GmbH">
// Photon Voice - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Component that should be attached to a networked PUN prefab that has 
// PhotonView. It will bind remote Recorder with local Speaker of the same 
// networked perfab. This component makes automatic voice stream routing easy 
// for players' characters/avatars.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace Photon.Voice.PUN
{
    using Pun;
    using UnityEngine;
    using Unity;

    /// <summary>
    /// Component that should be attached to a networked PUN prefab that has <see cref="PhotonView"/>. 
    /// It will bind remote <see cref="Recorder"/> with local <see cref="Speaker"/> of the same networked prefab. 
    /// This component makes automatic voice stream routing easy for players' characters/avatars.
    /// </summary>
    [AddComponentMenu("Photon Voice/Photon Voice View")]
    [RequireComponent(typeof(PhotonView))]
    [HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/voice-for-pun")]
    public class PhotonVoiceView : VoiceComponent
    {
        #region Private Fields

        private PhotonView photonView;

        [SerializeField]
        private Recorder recorderInUse;

        [SerializeField]
        private Speaker speakerInUse;

        #endregion

        #region Public Fields

        /// <summary> If true, a Recorder component will be added to the same GameObject if not found already. </summary>
        public bool AutoCreateRecorderIfNotFound;
        /// <summary> If true, PhotonVoiceNetwork.PrimaryRecorder will be used by this PhotonVoiceView </summary>
        public bool UsePrimaryRecorder;
        /// <summary> If true, a Speaker component will be setup to be used for the DebugEcho mode </summary>
        public bool SetupDebugSpeaker;

        #endregion

        #region Properties

        /// <summary> The Recorder component currently used by this PhotonVoiceView </summary>
        public Recorder RecorderInUse
        {
            get { return recorderInUse; }
            set
            {
                if (photonView.IsMine)
                {
                    if (recorderInUse != value || !IsRecorder)
                    {
                        recorderInUse = value;
                        IsRecorder = SetupRecorder(recorderInUse);
                    }
                }
                else if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("No need to set Recorder as the PhotonView does not belong to local player");
                }
            }
        }

        /// <summary> The Speaker component currently used by this PhotonVoiceView </summary>
        public Speaker SpeakerInUse
        {
            get { return speakerInUse; }
            set
            {
                if (SetupDebugSpeaker || !photonView.IsMine)
                {
                    if (speakerInUse != value || !IsSpeaker)
                    {
                        speakerInUse = value;
                        IsSpeaker = SetupSpeaker(speakerInUse);
                    }
                } else if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Speaker not set because the PhotonView does not belong to a remote player or SetupDebugSpeaker is disabled");
                }
            }
        }
        /// <summary> If true, this PhotonVoiceView is setup and ready to be used </summary>
        public bool IsSetup { get; protected set; }
        /// <summary> If true, this PhotonVoiceView has a Speaker setup for playback of received audio frames from remote audio source </summary>
        public bool IsSpeaker { get; protected set; }
        /// <summary> If true, this PhotonVoiceView has a Speaker that is currently playing received audio frames from remote audio source </summary>
        public bool IsSpeaking
        {
            get { return IsSpeaker && SpeakerInUse.IsPlaying; }
        }
        /// <summary> If true, this PhotonVoiceView has a Recorder setup for transmission of audio stream from local audio source </summary>
        public bool IsRecorder { get; protected set; }
        /// <summary> If true, this PhotonVoiceView has a Recorder that is currently transmitting audio stream from local audio source </summary>
        public bool IsRecording
        {
            get { return IsRecorder && RecorderInUse.IsCurrentlyTransmitting; }
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            photonView = GetComponent<PhotonView>();
        }

        private void OnEnable()
        {
            if (photonView.ViewID > 0)
            {
                Setup();
                if (IsSpeaker && !this.SpeakerInUse.IsLinked)
                {
                    PhotonVoiceNetwork.Instance.CheckLateLinking(this, photonView.ViewID);
                }
            }
        }

        private void Setup()
        {
            if (IsSetup)
            {
                return;
            }
            if (photonView.IsMine)
            {
                IsRecorder = SetupRecorder();
            }
            if (SetupDebugSpeaker || !photonView.IsMine)
            {
                IsSpeaker = SetupSpeaker();
            }
            IsSetup = true;
        }

        private bool SetupRecorder()
        {
            if (recorderInUse == null) // not manually assigned by user
            {
                if (UsePrimaryRecorder)
                {
                    recorderInUse = PhotonVoiceNetwork.Instance.PrimaryRecorder;
                }
                else
                {
                    Recorder[] recorders = GetComponentsInChildren<Recorder>();
                    if (recorders.Length > 0)
                    {
                        recorderInUse = recorders[0];
                        if (recorders.Length > 1 && this.Logger.IsWarningEnabled)
                        {
                            this.Logger.LogWarning("Multiple Recorder components found attached to the GameObject or its children");
                        }
                    }
                }
                if (recorderInUse == null)
                {
                    if (!AutoCreateRecorderIfNotFound)
                    {
                        return false;
                    }
                    recorderInUse = this.gameObject.AddComponent<Recorder>();
                }
            }
            return SetupRecorder(recorderInUse);
        }

        private bool SetupRecorder(Recorder recorder)
        {
            if (recorder == null)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot setup a null Recorder");
                }
                return false;
            }
            if (photonView.ViewID <= 0)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder setup cannot be done before assigning a valid ViewID to the PhotonView attached to the same GameObject as the PhotonVoiceView");
                }
                return false;
            }
            // check if already initialized
            if (recorder.IsInitialized)
            {
                if (recorder.UserData is int && photonView.ViewID == (int) recorder.UserData)
                {
                    return true;
                }
                recorder.UserData = photonView.ViewID;
                if (recorder.RequiresRestart)
                {
                    recorder.RestartRecording();
                }
                return true;
            }
            RecorderInUse.Init(PhotonVoiceNetwork.Instance.VoiceClient, photonView.ViewID);
            return true;
        }

        private bool SetupSpeaker()
        {
            if (speakerInUse == null) // not manually assigned by user
            {
                Speaker[] speakers = GetComponentsInChildren<Speaker>();
                if (speakers.Length > 0)
                {
                    speakerInUse = speakers[0];
                    if (speakers.Length > 1 && this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Multiple Speaker components found attached to the GameObject or its children");
                    }
                }
                if (speakerInUse == null)
                {
                    if (!PhotonVoiceNetwork.Instance.AutoCreateSpeakerIfNotFound)
                    {
                        return false;
                    }
                    if (PhotonVoiceNetwork.Instance.SpeakerPrefab != null)
                    {
                        GameObject go = Instantiate(PhotonVoiceNetwork.Instance.SpeakerPrefab, this.transform, false);
                        speakerInUse = go.GetComponentInChildren<Speaker>();
                        if (speakerInUse == null)
                        {
                            if (this.Logger.IsErrorEnabled)
                            {
                                this.Logger.LogError("SpeakerPrefab does not have a component of type Speaker in its hierarchy.");
                            }
                            Destroy(go);
                            return false;
                        }
                    }
                    else
                    {
                        speakerInUse = this.gameObject.AddComponent<Speaker>();
                        // get AudioSource and set spatialBlend
                        AudioSource audioSource = speakerInUse.GetComponentInChildren<AudioSource>();
                        audioSource.spatialBlend = 1.0f;
                        return true;
                    }
                }
            }
            return SetupSpeaker(speakerInUse);
        }

        private bool SetupSpeaker(Speaker speaker)
        {
            if (speaker == null)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot setup a null Speaker");
                }
                return false;
            }
            AudioSource audioSource = speaker.GetComponentInChildren<AudioSource>();
            if (audioSource == null)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Unexpected: no AudioSource found attached to the same GameObject as the Speaker component");
                }
                return false;
            }
            return true;
        }

        #endregion
    }
}
