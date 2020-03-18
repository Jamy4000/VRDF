using System.Collections.Generic;
using UnityEngine;

namespace Photon.Voice.Unity
{
    [RequireComponent(typeof(Recorder))]
    public class WebRtcAudioDsp : VoiceComponent
    {
        #region Private Fields

        [SerializeField]
        private bool aec = true;

        [SerializeField]
        private bool aecMobile;
        
        [SerializeField]
        private bool aecMobileComfortNoise;

        [SerializeField]
        private bool agc = true;

        [SerializeField]
        private bool vad = true;

        [SerializeField]
        private bool highPass;

        [SerializeField]
        private bool bypass;

        [SerializeField]
        private bool noiseSuppression;

        [SerializeField]
        private int reverseStreamDelayMs = 120;

        private int reverseChannels;
        private WebRTCAudioProcessor proc;

        private AudioOutCapture ac;
        private bool started;

        private static readonly Dictionary<AudioSpeakerMode, int> channelsMap = new Dictionary<AudioSpeakerMode, int>
        {
            #if !UNITY_2019_2_OR_NEWER
            {AudioSpeakerMode.Raw, 0},
            #endif
            {AudioSpeakerMode.Mono, 1},
            {AudioSpeakerMode.Stereo, 2},
            {AudioSpeakerMode.Quad, 4},
            {AudioSpeakerMode.Surround, 5},
            {AudioSpeakerMode.Mode5point1, 6},
            {AudioSpeakerMode.Mode7point1, 8},
            {AudioSpeakerMode.Prologic, 2}
        };

        private LocalVoice localVoice;
        private int outputSampleRate;

        private Recorder recorder;

        #endregion

        #region Properties

        public bool AEC
        {
            get { return this.aec; }
            set
            {
                if (value == this.aec)
                {
                    return;
                }
                if (value)
                {
                    this.aecMobile = false;
                }
                this.aec = value;
                if (this.proc != null)
                {
                    this.proc.AEC = this.aec;
                    this.proc.AECMobile = this.aecMobile;
                }
                this.SetOutputListener();
            }
        }

        public bool AECMobile // echo control mobile
        {
            get { return this.aecMobile; }
            set
            {
                if (value == this.aecMobile)
                {
                    return;
                }
                if (value)
                {
                    this.aec = false;
                }
                this.aecMobile = value;
                if (this.proc != null)
                {
                    this.proc.AEC = this.aec;
                    this.proc.AECMobile = this.aecMobile;
                }
                this.SetOutputListener();
            }
        }

        public bool AECMobileComfortNoise
        {
            get { return this.aecMobileComfortNoise; }
            set
            {
                if (value == this.aecMobileComfortNoise)
                {
                    return;
                }
                if (this.proc != null)
                {
                    this.proc.AECMComfortNoise = this.aecMobileComfortNoise;
                }
            }
        }

        public int ReverseStreamDelayMs
        {
            get { return this.reverseStreamDelayMs; }
            set
            {
                if (this.reverseStreamDelayMs == value)
                {
                    return;
                }
                this.reverseStreamDelayMs = value;
                if (this.proc != null)
                {
                    this.proc.AECStreamDelayMs = this.ReverseStreamDelayMs;
                } 
            }
        }

        public bool NoiseSuppression
        {
            get { return this.noiseSuppression; }
            set
            {
                if (value == this.noiseSuppression)
                {
                    return;
                }
                this.noiseSuppression = value;
                if (this.proc != null)
                {
                    this.proc.NoiseSuppression = this.noiseSuppression;
                }
            }
        }

        public bool HighPass
        {
            get { return this.highPass; }
            set
            {
                if (value == this.highPass)
                {
                    return;
                }
                this.highPass = value;
                if (this.proc != null)
                {
                    this.proc.HighPass = this.highPass;
                }
            }
        }

        public bool Bypass
        {
            get { return this.bypass; }
            set
            {
                if (value == this.bypass)
                {
                    return;
                }
                this.bypass = value;
                if (this.proc != null)
                {
                    this.proc.Bypass = this.bypass;
                }
            }
        }

        public bool AGC
        {
            get { return this.agc; }
            set
            {
                if (value == this.agc)
                {
                    return;
                }
                this.agc = value;
                if (this.proc != null)
                {
                    this.proc.AGC = this.agc;
                }
            }
        }

        public bool VAD
        {
            get { return this.vad; }
            set
            {
                if (value == this.vad)
                {
                    return;
                }
                this.vad = value;
                if (this.proc != null)
                {
                    this.proc.VAD = this.vad;
                }
            }
        }

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            AudioListener[] audioListeners = FindObjectsOfType<AudioListener>();
            AudioListener audioListener = null;
            for(int i=0; i < audioListeners.Length; i++)
            {
                if (audioListeners[i].gameObject.activeInHierarchy && audioListeners[i].enabled)
                {
                    audioListener = audioListeners[i];
                    break;
                }
            }
            if (!this.SetOrSwitchAudioListener(audioListener, false))
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("AudioListener and AudioOutCapture components are required");
                }
                this.enabled = false;
            }
            this.recorder = GetComponent<Recorder>();
            if (this.recorder == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("A Recorder component needs to be attached to the same GameObject");
                }
                this.enabled = false;
            }
        }

        private void OnEnable()
        {
            this.SetOutputListener();
        }

        private void OnDisable()
        {
            this.SetOutputListener(false);
        }

        private void SetOutputListener()
        {
            this.SetOutputListener(this.aec || this.aecMobile);
        }

        private void SetOutputListener(bool on)
        {
            if (this.ac != null && this.started != on && this.proc != null)
            {
                if (on)
                {
                    this.started = true;
                    this.ac.OnAudioFrame += this.OnAudioOutFrameFloat;
                }
                else
                {
                    this.started = false;
                    this.ac.OnAudioFrame -= this.OnAudioOutFrameFloat;
                }
            }
        }

        private void OnAudioOutFrameFloat(float[] data, int outChannels)
        {
            if (outChannels != this.reverseChannels)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("OnAudioOutFrame channel count {0} != initialized {1}.  Switching channels and restarting.", outChannels, this.reverseChannels);
                }
                this.reverseChannels = outChannels;
                this.Restart();
            }
            this.proc.OnAudioOutFrameFloat(data);
        }

        // Message sent by Recorder
        private void PhotonVoiceCreated(PhotonVoiceCreatedParams p)
        {
            if (!this.enabled)
            {
                return;
            }
            if (this.recorder != null && this.recorder.SourceType != Recorder.InputSourceType.Microphone)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("WebRtcAudioDsp should be used with Recorder.SourceType == Recorder.InputSourceType.Microphone only.");
                }
                this.enabled = false;
                return;
            }
            this.localVoice = p.Voice;
            if (this.localVoice.Info.Channels != 1)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Only mono audio signals supported.");
                }
                this.enabled = false;
                return;
            }
            if (!(this.localVoice is LocalVoiceAudioShort))
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Only short audio voice supported.");
                }
                this.enabled = false;
                return;
            }

            // can't access the AudioSettings properties in InitAEC if it's called from not main thread
            this.reverseChannels = channelsMap[AudioSettings.speakerMode];
            this.outputSampleRate = AudioSettings.outputSampleRate;
            this.Init();
            LocalVoiceAudioShort v = this.localVoice as LocalVoiceAudioShort;
            v.AddPostProcessor(this.proc);
            this.SetOutputListener();
            if (this.Logger.IsInfoEnabled)
            {
                this.Logger.LogInfo("Initialized");
            }
        }

        private void PhotonVoiceRemoved()
        {
            this.Reset();
        }

        private void OnDestroy()
        {
            this.Reset();
        }

        private void Reset()
        {
            this.SetOutputListener(false);
            if (this.proc != null)
            {
                this.proc.Dispose();
                this.proc = null;
            }
        }

        private void Restart()
        {
            this.SetOutputListener(false);
            this.Init();
            this.SetOutputListener();
        }

        private void Init()
        {
            this.proc = new WebRTCAudioProcessor(this.Logger, this.localVoice.Info.FrameSize, this.localVoice.Info.SamplingRate,
                this.localVoice.Info.Channels, this.outputSampleRate, this.reverseChannels);
            this.proc.AEC = this.AEC;
            this.proc.AECMobile = this.AECMobile;
            this.proc.AECMRoutingMode = 4;
            this.proc.AECMComfortNoise = this.AECMobileComfortNoise;
            this.proc.AECStreamDelayMs = this.ReverseStreamDelayMs;
            this.proc.HighPass = this.HighPass;
            this.proc.NoiseSuppression = this.NoiseSuppression;
            this.proc.AGC = this.AGC;
            this.proc.VAD = this.VAD;
            this.proc.Bypass = this.Bypass;
        }

        private bool SetOrSwitchAudioListener(AudioListener audioListener, bool extraChecks)
        {
            if (audioListener == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("audioListener passed is null or is being destroyed");
                }
                return false;
            }
            if (extraChecks)
            {
                if (audioListener.gameObject.activeInHierarchy)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("The GameObject to which the audioListener is attached is not active in hierarchy");
                    }
                    return false;
                }
                if (!audioListener.enabled)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("audioListener passed is disabled");
                    }
                    return false;
                }
            }
            AudioOutCapture audioOutCapture = audioListener.GetComponent<AudioOutCapture>();
            if (audioOutCapture == null)
            {
                audioOutCapture = audioListener.gameObject.AddComponent<AudioOutCapture>();
            }
            return this.SetOrSwitchAudioOutCapture(audioOutCapture, false);
        }


        private bool SetOrSwitchAudioOutCapture(AudioOutCapture audioOutCapture, bool extraChecks)
        {
            if (audioOutCapture == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("audioOutCapture passed is null or is being destroyed");
                }
                return false;
            }
            if (!audioOutCapture.enabled)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("audioOutCapture passed is disabled");
                }
                return false;
            }
            if (extraChecks)
            {
                if (audioOutCapture.gameObject.activeInHierarchy)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("The GameObject to which the audioOutCapture is attached is not active in hierarchy");
                    }
                    return false;
                }
                AudioListener audioListener = audioOutCapture.GetComponent<AudioListener>();
                if (audioListener == null)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("The AudioListener attached to the same GameObject as the audioOutCapture is null or being destroyed");
                    }
                    return false;
                }
                if (!audioListener.enabled)
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("The AudioListener attached to the same GameObject as the audioOutCapture is disabled");
                    }
                    return false;
                }
            }
            if (this.ac != null)
            {
                if (this.ac != audioOutCapture)
                {
                    if (this.started)
                    {
                        this.ac.OnAudioFrame -= this.OnAudioOutFrameFloat;
                    }
                }
                else
                {
                    if (this.Logger.IsErrorEnabled)
                    {
                        this.Logger.LogError("The same audioOutCapture is being used already");
                    }
                    return false;
                }
            }
            this.ac = audioOutCapture;
            if (this.started)
            {
                this.ac.OnAudioFrame += this.OnAudioOutFrameFloat;
            }
            return true;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the AudioListener to be used with this WebRtcAudioDsp
        /// </summary>
        /// <param name="audioListener">The audioListener to be used</param>
        /// <returns>Success or failure</returns>
        public bool SetOrSwitchAudioListener(AudioListener audioListener)
        {
            return this.SetOrSwitchAudioListener(audioListener, true);
        }

        /// <summary>
        /// Set the AudioOutCapture to be used with this WebRtcAudioDsp
        /// </summary>
        /// <param name="audioOutCapture">The audioOutCapture to be used</param>
        /// <returns>Success or failure</returns>
        public bool SetOrSwitchAudioOutCapture(AudioOutCapture audioOutCapture)
        {
            return this.SetOrSwitchAudioOutCapture(audioOutCapture, true);
        }

        #endregion
    }
}