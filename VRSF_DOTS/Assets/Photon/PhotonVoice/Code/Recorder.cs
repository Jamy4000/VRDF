// ----------------------------------------------------------------------------
// <copyright file="Recorder.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Component representing outgoing audio stream in scene.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Photon.Voice.Unity
{
    /// <summary>
    /// Component representing outgoing audio stream in scene.
    /// </summary>
    [AddComponentMenu("Photon Voice/Recorder")]
    [HelpURL("https://doc.photonengine.com/en-us/voice/v2/getting-started/recorder")]
    public class Recorder : VoiceComponent
    {
        #region Private Fields

        [SerializeField]
        private bool voiceDetection;

        [SerializeField]
        private float voiceDetectionThreshold = 0.01f;

        [SerializeField]
        private int voiceDetectionDelayMs = 500;

        private object userData;

        private LocalVoice voice = LocalVoiceAudioDummy.Dummy;

        #if UNITY_EDITOR
        [SerializeField]
        #endif
        private string unityMicrophoneDevice;

        #if UNITY_EDITOR
        [SerializeField]
        #endif
        private int photonMicrophoneDeviceId = -1;

        private IAudioDesc inputSource;

        private VoiceClient client;

        private bool forceShort;

        [SerializeField]
        [FormerlySerializedAs("audioGroup")]
        private byte interestGroup;

        [SerializeField]
        private bool debugEchoMode;

        [SerializeField]
        private bool reliableMode;

        [SerializeField]
        private bool encrypt;

        [SerializeField]
        private bool transmitEnabled;

        [SerializeField]
        private POpusCodec.Enums.SamplingRate samplingRate = POpusCodec.Enums.SamplingRate.Sampling24000;

        [SerializeField]
        private OpusCodec.FrameDuration frameDuration = OpusCodec.FrameDuration.Frame20ms;

        [SerializeField]
        private int bitrate = 30000;

        [SerializeField]
        private InputSourceType sourceType;

        [SerializeField]
        private MicType microphoneType;

        [SerializeField]
        private AudioClip audioClip;

        [SerializeField]
        private bool loopAudioClip = true;

        private bool isRecording;

        private Func<IAudioDesc> inputFactory;

        private static AudioInEnumerator photonMicrophoneEnumerator;

        [SerializeField]
        private bool reactOnSystemChanges;

        private bool subscribedToSystemChanges;

        [SerializeField]
        private bool autoStart = true;

        #if UNITY_IOS
        [SerializeField] 
        private IOS.AudioSessionParameters audioSessionParameters = IOS.AudioSessionParametersPresets.Game;
        #if UNITY_EDITOR
        [SerializeField]
        private bool useCustomAudioSessionParameters;
        [SerializeField] 
        private int audioSessionPresetIndex;
        #endif
        #endif

        #endregion

        #region Properties

        /// <summary>Enumerator for the available microphone devices gathered by the Photon plugin.</summary>
        public static AudioInEnumerator PhotonMicrophoneEnumerator
        {
            get
            {
                if (photonMicrophoneEnumerator == null)
                {
                    VoiceLogger logger = new VoiceLogger("AudioInEnumerator");
                    photonMicrophoneEnumerator = new AudioInEnumerator(logger);
                    if (photonMicrophoneEnumerator.Error != null && logger.IsErrorEnabled)
                    {
                        logger.LogError("PhotonVoice: Can't create instance of AudioInEnumerator: {0}", photonMicrophoneEnumerator.Error);
                    }
                }
                return photonMicrophoneEnumerator;
            }
        }

        /// <summary>If true, this Recorder has been initialized and is ready to transmit to remote clients. Otherwise call <see cref="Init"/>.</summary>
        public bool IsInitialized
        {
            get { return this.client != null; }
        }

        [Obsolete("Renamed to RequiresRestart")]
        public bool RequiresInit
        {
            get { return this.RequiresRestart; }
        }

        /// <summary>Returns true if something has changed in the Recorder while recording that won't take effect unless recording is restarted using <see cref="RestartRecording"/>.</summary>
        /// <remarks>Think of this as a "isDirty" flag.</remarks>
        public bool RequiresRestart { get; protected set; }

        /// <summary>If true, audio transmission is enabled.</summary>
        public bool TransmitEnabled
        {
            get { return this.transmitEnabled; }
            set
            {
                if (value != this.transmitEnabled)
                {
                    this.transmitEnabled = value;
                    if (this.voice != LocalVoiceAudioDummy.Dummy)
                    {
                        this.voice.TransmitEnabled = value;
                    }
                }
            }
        }

        /// <summary>If true, voice stream is sent encrypted.</summary>
        public bool Encrypt
        {
            get { return encrypt; }
            set
            {
                if (encrypt == value)
                {
                    return;
                }
                encrypt = value;
                voice.Encrypt = value;
            }
        }

        /// <summary>If true, outgoing stream routed back to client via server same way as for remote client's streams.</summary>
        public bool DebugEchoMode
        {
            get
            {
                return debugEchoMode;
            }
            set
            {
                if (debugEchoMode == value)
                {
                    return;
                }
                if (this.InterestGroup != 0)
                {
                    if (this.Logger.IsWarningEnabled)
                    {
                        this.Logger.LogWarning("Cannot enable DebugEchoMode when InterestGroup value ({0}) is different than 0.", this.InterestGroup);
                    }
                    return;
                }
                debugEchoMode = value;
                voice.DebugEchoMode = value;
            }
        }

        /// <summary>If true, stream data sent in reliable mode.</summary>
        public bool ReliableMode
        {
            get 
            { 
                return reliableMode;
            }
            set
            {
                if (this.voice != LocalVoiceAudioDummy.Dummy)
                {
                    this.voice.Reliable = value;
                }
                reliableMode = value;
            }
        }

        /// <summary>If true, voice detection enabled.</summary>
        public bool VoiceDetection
        {
            get
            {
                this.GetStatusFromDetector();
                return this.voiceDetection;
            }
            set
            {
                this.voiceDetection = value;
                if (this.VoiceDetector != null)
                {
                    this.VoiceDetector.On = value;
                }
            }
        }

        /// <summary>Voice detection threshold (0..1, where 1 is full amplitude).</summary>
        public float VoiceDetectionThreshold
        {
            get
            {
                this.GetThresholdFromDetector();
                return this.voiceDetectionThreshold;
            }
            set
            {
                if (this.voiceDetectionThreshold.Equals(value))
                {
                    return;
                }
                if (value < 0f || value > 1f)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Value out of range: VAD Threshold needs to be between [0..1], requested value: {0}", value);
                    }
                    return;
                }
                this.voiceDetectionThreshold = value;
                if (this.VoiceDetector != null)
                {
                    this.VoiceDetector.Threshold = this.voiceDetectionThreshold;
                }
            }
        }

        /// <summary>Keep detected state during this time after signal level dropped below threshold. Default is 500ms</summary>
        public int VoiceDetectionDelayMs
        {
            get
            {
                this.GetActivityDelayFromDetector();
                return this.voiceDetectionDelayMs;
            }
            set
            {
                if (this.voiceDetectionDelayMs == value)
                {
                    return;
                }
                this.voiceDetectionDelayMs = value;
                if (this.VoiceDetector != null)
                {
                    this.VoiceDetector.ActivityDelayMs = value;
                }
            }
        }

        /// <summary>Custom user object to be sent in the voice stream info event.</summary>
        public object UserData
        {
            get { return this.userData; }
            set
            {
                if (this.userData != value)
                {
                    this.userData = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "UserData");
                        }
                    }
                }
            }
        }

        /// <summary>Set the method returning new Voice.IAudioDesc instance to be assigned to a new voice created with Source set to Factory</summary>
        public Func<IAudioDesc> InputFactory
        {
            get { return this.inputFactory; }
            set
            {
                if (this.inputFactory != value)
                {
                    this.inputFactory = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "InputFactory");
                        }
                    }
                }
            }
        }

        /// <summary>Returns voice activity detector for recorder's audio stream.</summary>
        public AudioUtil.IVoiceDetector VoiceDetector
        {
            get { return this.voiceAudio != null ? this.voiceAudio.VoiceDetector : null; }
        }

        /// <summary>Set or get Unity microphone device used for streaming.</summary>
        public string UnityMicrophoneDevice
        {
            get
            {
                if (!string.IsNullOrEmpty(this.unityMicrophoneDevice) && !Microphone.devices.Contains(this.unityMicrophoneDevice))
                {
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("\"{0}\" is not a valid Unity microphone device, switching to default (null)", this.unityMicrophoneDevice);
                    }
                    this.unityMicrophoneDevice = null;
                }
                return this.unityMicrophoneDevice;
            }
            set
            {
                if (!string.IsNullOrEmpty(value) && !Microphone.devices.Contains(value))
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("\"{0}\" is not a valid Unity microphone device", value);
                    }
                    return;
                }

                if (value == null && this.unityMicrophoneDevice != null || value != null && !value.Equals(unityMicrophoneDevice))
                {
                    this.unityMicrophoneDevice = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "UnityMicrophoneDevice");
                        }
                    }
                }

            }
        }

        /// <summary>Set or get photon microphone device used for streaming.</summary>
        public int PhotonMicrophoneDeviceId
        {
            get
            {
                if (!PhotonMicrophoneEnumerator.IsSupported)
                {
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Photon microphone device IDs are not supported on this platform");
                    }
                    this.photonMicrophoneDeviceId = -1;
                }
                else if (!PhotonMicrophoneEnumerator.IDIsValid(this.photonMicrophoneDeviceId))
                {
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("\"{0}\" is not a valid Photon microphone device, switching to default (-1)", this.photonMicrophoneDeviceId);
                    }
                    this.photonMicrophoneDeviceId = -1;
                }
                return this.photonMicrophoneDeviceId;
            }
            set
            {
                if (!PhotonMicrophoneEnumerator.IsSupported)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Setting a Photon microphone device ID is not supported on this platform");
                    }
                    return;
                }
                if (!PhotonMicrophoneEnumerator.IDIsValid(value))
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("\"{0}\" is not a valid Photon microphone device", value);
                    }
                    return;
                }

                if (this.photonMicrophoneDeviceId != value)
                {
                    this.photonMicrophoneDeviceId = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "PhotonMicrophoneDeviceId");
                        }
                    }
                }

            }
        }

        /// <summary>Target interest group that will receive transmitted audio.</summary>
        /// <remarks>If AudioGroup != 0, recorder's audio data is sent only to clients listening to this group.</remarks>
        [Obsolete("Use InterestGroup instead")]
        public byte AudioGroup
        {
            get { return this.InterestGroup; }
            set { this.InterestGroup = value; }
        }

        /// <summary>Target interest group that will receive transmitted audio.</summary>
        /// <remarks>If InterestGroup != 0, recorder's audio data is sent only to clients listening to this group.</remarks>
        public byte InterestGroup
        {
            get { return interestGroup; }
            set
            {
                if (interestGroup == value)
                {
                    return;
                }
                interestGroup = value;
                voice.InterestGroup = value;
            }
        }

        /// <summary>Returns true if audio stream broadcasts.</summary>
        public bool IsCurrentlyTransmitting
        {
            get { return voice.IsCurrentlyTransmitting; }
        }

        /// <summary>Level meter utility.</summary>
        public AudioUtil.ILevelMeter LevelMeter
        {
            get { return this.voiceAudio != null ? this.voiceAudio.LevelMeter : null; }
        }

        /// <summary>If true, voice detector calibration is in progress.</summary>
        public bool VoiceDetectorCalibrating { get { return voiceAudio != null && voiceAudio.VoiceDetectorCalibrating; } }

        protected ILocalVoiceAudio voiceAudio { get { return this.voice as ILocalVoiceAudio; } }

        /// <summary>Audio data source.</summary>
        public InputSourceType SourceType
        {
            get { return this.sourceType; }
            set
            {
                if (this.sourceType != value)
                {
                    this.sourceType = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "Source");
                        }
                    }
                }
            }
        }

        /// <summary>Which microphone API to use when the Source is set to Microphone.</summary>
        public MicType MicrophoneType
        {
            get { return this.microphoneType; }
            set
            {
                if (this.microphoneType != value)
                {
                    #if !UNITY_STANDALONE_OSX && !UNITY_STANDALONE_WIN && !UNITY_ANDROID && !UNITY_IOS
                    if (value == MicType.Photon)
                    {
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("Photon microphone type is not supported on this platform");
                        }
                        return;
                    }
                    #endif
                    this.microphoneType = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "MicrophoneType");
                        }
                    }
                }
            }
        }

        #pragma warning disable 618
        /// <summary>Force creation of 'short' pipeline and convert audio data to short for 'float' audio sources.</summary>
        [Obsolete("No longer used. Implicit conversion is done internally when needed.")]
        public SampleTypeConv TypeConvert { get; set; }
        #pragma warning restore 618

        /// <summary>Source audio clip.</summary>
        public AudioClip AudioClip
        {
            get { return this.audioClip; }
            set
            {
                if (this.audioClip != value)
                {
                    this.audioClip = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "AudioClip");
                        }
                    }
                }
            }
        }

        /// <summary>Loop playback for audio clip sources.</summary>
        public bool LoopAudioClip
        {
            get { return this.loopAudioClip; }
            set
            {
                if (this.loopAudioClip != value)
                {
                    this.loopAudioClip = value;
                    AudioClipWrapper wrapper = this.inputSource as AudioClipWrapper;
                    if (wrapper != null)
                    {
                        wrapper.Loop = value;
                    }
                    else if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "LoopAudioClip");
                        }
                    }
                }
            }
        }

        /// <summary>Outgoing audio stream sampling rate.</summary>
        public POpusCodec.Enums.SamplingRate SamplingRate
        {
            get { return this.samplingRate; }
            set
            {
                if (this.samplingRate != value)
                {
                    this.samplingRate = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "SamplingRate");
                        }
                    }
                }
            }
        }

        /// <summary>Outgoing audio stream encoder delay.</summary>
        public OpusCodec.FrameDuration FrameDuration
        {
            get { return this.frameDuration; }
            set
            {
                if (this.frameDuration != value)
                {
                    this.frameDuration = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "FrameDuration");
                        }
                    }
                }
            }
        }

        /// <summary>Outgoing audio stream bitrate.</summary>
        public int Bitrate
        {
            get { return this.bitrate; }
            set
            {
                if (this.bitrate != value)
                {
                    this.bitrate = value;
                    if (this.IsRecording)
                    {
                        this.RequiresRestart = true;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "Bitrate");
                        }
                    }
                }
            }
        }

        /// <summary>Gets or sets whether this Recorder is actively recording audio to be transmitted.</summary>
        public bool IsRecording
        {
            get
            {
                return this.isRecording;
            }
            set
            {
                if (this.isRecording != value)
                {
                    if (this.isRecording)
                    {
                        this.StopRecording();
                    }
                    else
                    {
                        this.StartRecording();
                    }
                }
            }
        }

        public bool ReactOnSystemChanges
        {
            get { return this.reactOnSystemChanges; }
            set
            {
                if (this.reactOnSystemChanges != value)
                {
                    this.reactOnSystemChanges = value;
                    if (this.IsRecording && this.reactOnSystemChanges != this.subscribedToSystemChanges)
                    {
                        if (this.reactOnSystemChanges)
                        {
                            AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigChanged;
                        }
                        else if (this.subscribedToSystemChanges)
                        {
                            AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigChanged;
                        }
                        this.subscribedToSystemChanges = this.reactOnSystemChanges;
                    }
                }
            }
        }

        /// <summary> If true, automatically start recording when initialized. </summary>
        public bool AutoStart
        {
            get { return this.autoStart; }
            set { this.autoStart = value; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes the Recorder component to be able to transmit audio.
        /// </summary>
        /// <param name="voiceClient">The VoiceClient to be used with this Recorder.</param>
        /// <param name="customObj">Optional user data object to be transmitted with the voice stream info</param>
        public void Init(VoiceClient voiceClient, object customObj = null)
        {
            if (this.IsInitialized)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder already initialized.");
                }
                return;
            }

            if (voiceClient == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("voiceClient is null.");
                }
                return;
            }

            this.client = voiceClient;
            this.userData = customObj;

            if (this.AutoStart)
            {
                this.StartRecording();
            }
        }

        [Obsolete("Renamed to RestartRecording")]
        public void ReInit()
        {
            this.RestartRecording();
        }

        /// <summary>
        /// Restarts recording if something has changed that requires this.
        /// </summary>
        public void RestartRecording()
        {
            if (!this.RequiresRestart)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder does not require restart.");
                }
                return;
            }
            this.StopRecording();
            this.StartRecording();
        }

        /// <summary>Trigger voice detector calibration process.
        /// While calibrating, keep silence. Voice detector sets threshold basing on measured backgroud noise level.
        /// </summary>
        /// <param name="durationMs">Duration of calibration in milliseconds.</param>
        public void VoiceDetectorCalibrate(int durationMs, Action<float> detectionEndedCallback = null)
        {
            if (this.voiceAudio != null)
            {
                this.voiceAudio.VoiceDetectorCalibrate(durationMs, newThreshold =>
                {
                    this.GetThresholdFromDetector();
                    if (detectionEndedCallback != null)
                    {
                        detectionEndedCallback(this.voiceDetectionThreshold);
                    }
                });
            }
        }

        /// <summary> Starts recording. </summary>
        public void StartRecording()
        {
            if (this.IsRecording)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder is already started.");
                }
                return;
            }
            if (!this.IsInitialized)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recording can't be started if Recorder is not initialized. Call Recorder.Init(VoiceClient, Object) first.");
                }
                return;
            }
            this.Setup();
        }

        /// <summary> Stops recording. </summary>
        public void StopRecording()
        {
            if (!this.IsRecording)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Recorder is not started.");
                }
                return;
            }
            this.RemoveVoice(true);
        }

        #if UNITY_IOS
        /// <summary>
        /// Sets the AudioSessionParameters for iOS audio initialization when Photon MicrophoneType is used.
        /// </summary>
        /// <param name="asp">You can use custom value or one from presets, <see cref="IOS.AudioSessionParametersPresets"/></param>
        /// <returns>If a change has been made.</returns>
        public bool SetIosAudioSessionParameters(IOS.AudioSessionParameters asp)
        {
            return this.SetIosAudioSessionParameters(asp.Category, asp.Mode, asp.CategoryOptions);
        }
        /// <summary>
        /// Sets the AudioSessionParameters for iOS audio initialization when Photon MicrophoneType is used.
        /// </summary>
        /// <param name="category">Audio session category to be used.</param>
        /// <param name="mode">Audio session mode to be used.</param>
        /// <param name="options">Audio session category options to be used</param>
        /// <returns>If a change has been made.</returns>
        public bool SetIosAudioSessionParameters(IOS.AudioSessionCategory category, IOS.AudioSessionMode mode, IOS.AudioSessionCategoryOption[] options)
        {
            int opt = 0;
            if (options != null)
            {
                for (int i = 0; i < options.Length; i++)
                {
                    opt |= (int)options[i];
                }
            }
            if (this.audioSessionParameters.Category != category || 
                this.audioSessionParameters.Mode != mode ||
                this.audioSessionParameters.CategoryOptionsToInt() != opt)
            {
                this.audioSessionParameters.Category = category;
                this.audioSessionParameters.Mode = mode;
                this.audioSessionParameters.CategoryOptions = options;
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Changing iOS audioSessionParameters = {0}", audioSessionParameters);
                }
                if (this.IsRecording)
                {
                    this.RequiresRestart = true;
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Recorder.{0} changed, Recorder requires restart for this to take effect.", "iOSAudioSessionParameters");
                    }
                }
                return true;
            }
            return false;
        }

        #endif

        #endregion

        #region Private Methods

        private void Setup()
        {
            WebRtcAudioDsp dsp = gameObject.GetComponent<WebRtcAudioDsp>();
            if (dsp != null && dsp.isActiveAndEnabled)
            {
                forceShort = true;
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Type Conversion set to Short. Audio samples will be converted if source samples types differ.");
                }
            }
            else if (forceShort)
            {
                forceShort = false;
                if (this.Logger.IsInfoEnabled)
                {
                    this.Logger.LogInfo("Type Conversion disabled. Audio samples will not be converted if source samples types differ.");
                }
            }
            this.voice = CreateLocalVoiceAudioAndSource();
            if (this.voice == LocalVoiceAudioDummy.Dummy)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Local input source setup and voice stream creation failed. No recording or transmission will be happening. See previous error log messages for more details.");
                }
                return;
            }
            if (this.ReactOnSystemChanges && !this.subscribedToSystemChanges)
            {
                AudioSettings.OnAudioConfigurationChanged += this.OnAudioConfigChanged;
                this.subscribedToSystemChanges = true;
            }
            if (this.VoiceDetector != null)
            {
                this.VoiceDetector.Threshold = this.voiceDetectionThreshold;
                this.VoiceDetector.ActivityDelayMs = this.voiceDetectionDelayMs;
                this.VoiceDetector.On = this.voiceDetection;
            }
            this.voice.InterestGroup = this.InterestGroup;
            this.voice.DebugEchoMode = DebugEchoMode;
            this.voice.Encrypt = Encrypt;
            this.voice.Reliable = this.ReliableMode;
            this.RequiresRestart = false;
            this.isRecording = true;
            SendPhotonVoiceCreatedMessage();
            this.voice.TransmitEnabled = this.TransmitEnabled;
        }

        private LocalVoice CreateLocalVoiceAudioAndSource()
        {
            switch (SourceType)
            {
                case InputSourceType.Microphone:
                {
                    if (this.MicrophoneType == MicType.Photon)
                    {
                        #if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX || UNITY_EDITOR_WIN
                        var hwMicDev = this.PhotonMicrophoneDeviceId;
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Setting recorder's source to Photon microphone device [{0}] \"{1}\"", hwMicDev, PhotonMicrophoneEnumerator.NameAtIndex(hwMicDev));
                        }
                        #else
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Setting recorder's source to Photon microphone device");
                        }
                        #endif
                        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR || UNITY_EDITOR_WIN
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Setting recorder's source to WindowsAudioInPusher");
                        }
                        inputSource = new Windows.WindowsAudioInPusher(hwMicDev, this.Logger);
                        #elif UNITY_IOS && !UNITY_EDITOR
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Setting recorder's source to IOS.AudioInPusher with session {0}", audioSessionParameters);
                        }
                        inputSource = new IOS.AudioInPusher(audioSessionParameters, this.Logger);
                        #elif UNITY_STANDALONE_OSX && !UNITY_EDITOR || UNITY_EDITOR_OSX
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Setting recorder's source to MacOS.AudioInPusher");
                        }
                        inputSource = new MacOS.AudioInPusher(hwMicDev, this.Logger);
                        #elif UNITY_ANDROID && !UNITY_EDITOR
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("Setting recorder's source to UnityAndroidAudioInAEC");
                        }
                        inputSource = new UnityAndroidAudioInAEC(this.Logger);
                        #else
                        inputSource = new AudioDesc(0, 0, "Photon microphone type is not supported for the current platform.");
                        #endif
                        if (inputSource.Error == null)
                        {
                            break;
                        }
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("Photon microphone input source creation failure: {0}. Falling back to Unity microphone", inputSource.Error);
                        }
                    }
                    if (Microphone.devices.Length < 1)
                    {
                        if (this.Logger.IsInfoEnabled)
                        {
                            this.Logger.LogInfo("No Microphone");
                        }
                        return LocalVoiceAudioDummy.Dummy;
                    }
                    var micDev = this.UnityMicrophoneDevice;
                    if (this.Logger.IsInfoEnabled)
                    {
                        this.Logger.LogInfo("Setting recorder's source to Unity microphone device {0}", micDev);
                    }
                    // mic can ignore passed sampling rate and set its own
                    inputSource = new MicWrapper(micDev, (int)SamplingRate, this.Logger);
                    if (inputSource.Error != null && this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("Unity microphone input source creation failure: {0}.", inputSource.Error);
                    }
                }
                    break;
                case InputSourceType.AudioClip:
                {
                    if (AudioClip == null)
                    {
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("AudioClip property must be set for AudioClip audio source");
                        }
                        return LocalVoiceAudioDummy.Dummy;
                    }
                    inputSource = new AudioClipWrapper(AudioClip); // never fails, no need to check Error
                    if (this.LoopAudioClip)
                    {
                        ((AudioClipWrapper)inputSource).Loop = true;
                    }
                }
                    break;
                case InputSourceType.Factory:
                {
                    if (InputFactory == null)
                    {
                        if (this.Logger.IsErrorEnabled)
                        {
                            this.Logger.LogError("Recorder.InputFactory must be specified if Recorder.Source set to Factory");
                        }
                        return LocalVoiceAudioDummy.Dummy;
                    }
                    inputSource = InputFactory();
                    if (inputSource.Error != null && this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("InputFactory creation failure: {0}.", inputSource.Error);
                    }
                }
                    break;
                default:
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("unknown Source value {0}", SourceType);
                    }
                    return LocalVoiceAudioDummy.Dummy;
            }
            if (this.inputSource == null || this.inputSource.Error != null)
            {
                return LocalVoiceAudioDummy.Dummy;
            }
            VoiceInfo voiceInfo = VoiceInfo.CreateAudioOpus(SamplingRate, inputSource.Channels, FrameDuration, Bitrate, UserData);
            return client.CreateLocalVoiceAudioFromSource(voiceInfo, inputSource, forceShort);
        }

        protected virtual void SendPhotonVoiceCreatedMessage()
        {
            gameObject.SendMessage("PhotonVoiceCreated", new Unity.PhotonVoiceCreatedParams { Voice = this.voice, AudioDesc = this.inputSource }, SendMessageOptions.DontRequireReceiver);
        }

        private void OnDestroy()
        {
            // no need to send PhotonVoiceRemoved since object is destroyed
            this.RemoveVoice(false);
        }

        private void RemoveVoice(bool sendUnityMsg)
        {
            if (this.subscribedToSystemChanges)
            {
                AudioSettings.OnAudioConfigurationChanged -= this.OnAudioConfigChanged;
                this.subscribedToSystemChanges = false;
            }
            this.GetThresholdFromDetector();
            this.GetStatusFromDetector();
            this.GetActivityDelayFromDetector();
            if (this.voice != LocalVoiceAudioDummy.Dummy)
            {
                this.voice.RemoveSelf();
            }
            if (this.inputSource != null)
            {
                this.inputSource.Dispose();
                this.inputSource = null;
            }
            if (sendUnityMsg)
            {
                this.gameObject.SendMessage("PhotonVoiceRemoved", SendMessageOptions.DontRequireReceiver);
            }
            this.isRecording = false;
            this.RequiresRestart = false;
        }

        private void OnAudioConfigChanged(bool deviceWasChanged)
        {
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("OnAudioConfigChanged deviceWasChange={0}", deviceWasChanged);
            }
            if (deviceWasChanged && this.IsRecording)
            {
                this.RequiresRestart = true;
                PhotonMicrophoneEnumerator.Refresh();
                this.RestartRecording();
            }
        }

        private void GetThresholdFromDetector()
        {
            if (this.IsRecording && this.VoiceDetector != null && !this.voiceDetectionThreshold.Equals(this.VoiceDetector.Threshold))
            {
                if (this.VoiceDetector.Threshold <= 1f && this.VoiceDetector.Threshold >= 0f)
                {
                    if (this.Logger.IsDebugEnabled)
                    {
                        this.Logger.LogDebug("VoiceDetectionThreshold automatically changed from {0} to {1}", this.voiceDetectionThreshold, this.VoiceDetector.Threshold);
                    }
                    this.voiceDetectionThreshold = this.VoiceDetector.Threshold;
                }
                else if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("VoiceDetector.Threshold has unexpected value {0}", this.VoiceDetector.Threshold);
                }
            }
        }

        private void GetActivityDelayFromDetector()
        {
            if (this.IsRecording && this.VoiceDetector != null && this.voiceDetectionDelayMs != this.VoiceDetector.ActivityDelayMs)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("VoiceDetectionDelayMs automatically changed from {0} to {1}", this.voiceDetectionDelayMs, this.VoiceDetector.ActivityDelayMs);
                }
                this.voiceDetectionDelayMs = this.VoiceDetector.ActivityDelayMs;
            }
        }

        private void GetStatusFromDetector()
        {
            if (this.IsRecording && this.VoiceDetector != null && this.voiceDetection != this.VoiceDetector.On)
            {
                if (this.Logger.IsDebugEnabled)
                {
                    this.Logger.LogDebug("VoiceDetection automatically changed from {0} to {1}", this.voiceDetection, this.VoiceDetector.On);
                }
                this.voiceDetection = this.VoiceDetector.On;
            }
        }

        #endregion

        public enum InputSourceType
        {
            Microphone,
            AudioClip,
            Factory
        }

        public enum MicType
        {
            Unity,
            Photon
        }

        [Obsolete("No longer needed. Implicit conversion is done internally when needed.")]
        public enum SampleTypeConv
        {
            None,
            Short
        }
        
        [Obsolete("Use Photon.Voice.Unity.PhotonVoiceCreatedParams")]
        public class PhotonVoiceCreatedParams : Unity.PhotonVoiceCreatedParams
        {
        }
    }
}