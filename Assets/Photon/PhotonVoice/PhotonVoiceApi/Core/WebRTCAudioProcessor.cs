#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
#define DLL_IMPORT_INTERNAL
#endif
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
namespace Photon.Voice
{
    public class WebRTCAudioProcessor : WebRTCAudioLib, IProcessor<short>
    {
        int reverseStreamDelayMs;
        bool aec;
        bool aecm;
        int aecmRoutingMode;
        bool aecmComfortNoise;
        bool highPass;
        bool ns;
        bool agc;
        bool vad;
        public int AECStreamDelayMs { set { if (reverseStreamDelayMs != value) { reverseStreamDelayMs = value; if (proc != IntPtr.Zero) setParam(Param.REVERSE_STREAM_DELAY_MS, value); } } }
        public bool AEC
        {
            set
            {
                if (aec != value)
                {
                    aec = value;
                    InitReverseStream();
                    if (proc != IntPtr.Zero) setParam(Param.AEC, aec ? 1 : 0);                    
                    aecm = aec ? false : aecm;
                }
            }
        }        
        public bool AECMobile
        {
            set
            {
                if (aecm != value)
                {
                    aecm = value;
                    InitReverseStream();
                    if (proc != IntPtr.Zero) setParam(Param.AECM, aecm ? 1 : 0);
                    aec = aecm ? false : aec;
                }
            }
        }
        public int AECMRoutingMode { set { if (aecmRoutingMode != value) { aecmRoutingMode = value; if (proc != IntPtr.Zero) setParam(Param.AECM_ROUTING_MODE, value); } } }
        public bool AECMComfortNoise { set { if (aecmComfortNoise != value) { aecmComfortNoise = value; if (proc != IntPtr.Zero) setParam(Param.AECM_COMFORT_NOISE, value ? 1 : 0); } } }
        public bool HighPass { set { if (highPass != value) { highPass = value; if (proc != IntPtr.Zero) setParam(Param.HIGH_PASS_FILTER, value ? 1 : 0); } } }
        public bool NoiseSuppression { set { if (ns != value) { ns = value; if (proc != IntPtr.Zero) setParam(Param.NS, value ? 1 : 0); } } }
        public bool AGC { set { if (agc != value) { agc = value; if (proc != IntPtr.Zero) setParam(Param.AGC, value ? 1 : 0); } } }
        public bool VAD { set { if (vad != value) { vad = value; if (proc != IntPtr.Zero) setParam(Param.VAD, value ? 1 : 0); } } }
        public bool Bypass { set; private get; }
        int inFrameSize; // frames passed to Process
        int processFrameSize; // frames passed to webrtc_audio_processor_process
        int samplingRate; // input sampling rate (the same for Process and webrtc_audio_processor_process)
        int channels;
        IntPtr proc;
        bool disposed;
        
        Framer<float> reverseFramer;
        short[] reverseBuf;
        int reverseSamplingRate;
        int reverseChannels;
        ILogger logger;
        // audio parameters supported by webrtc
        const int supportedFrameLenMs = 10;
        int[] supportedSamplingRates = {  8000, 16000, 32000, 48000 };
        public WebRTCAudioProcessor(ILogger logger, int frameSize, int samplingRate, int channels, int reverseSamplingRate, int reverseChannels)
        {
            bool ok = false;
            foreach(var s in supportedSamplingRates)
            {
                if (samplingRate == s)
                {
                    ok = true;
                    break;
                }
            }
            if (!ok)
            {
                logger.LogError("WebRTCAudioProcessor: input sampling rate ({0}) must be 8000, 16000, 32000 or 48000", samplingRate);
                disposed = true;
                return;
            }            
            this.logger = logger;
            this.inFrameSize = frameSize;
            this.processFrameSize = samplingRate * supportedFrameLenMs / 1000;
            if (this.inFrameSize / this.processFrameSize * this.processFrameSize != this.inFrameSize)
            {
                logger.LogError("WebRTCAudioProcessor: input frame size ({0} samples / {1} ms) must be equal to or N times more than webrtc processing frame size ({2} samples / 10 ms)", this.inFrameSize, 1000f * this.inFrameSize / samplingRate, processFrameSize);
                disposed = true;
                return;
            }            
            this.samplingRate = samplingRate;
            this.channels = channels;            
            this.reverseSamplingRate = reverseSamplingRate;
            this.reverseChannels = reverseChannels;
            this.proc = webrtc_audio_processor_create(samplingRate, channels, this.processFrameSize, samplingRate /* reverseSamplingRate to be converted */, reverseChannels);
            setConfigParam(ConfigParam.AEC_DELAY_AGNOSTIC, 1);
            setConfigParam(ConfigParam.AEC_EXTENDED_FILTER, 1);
            webrtc_audio_processor_init(this.proc);
            if (this.inFrameSize != this.processFrameSize)
            {
                logger.LogWarning("WebRTCAudioProcessor: Frame size is {0} ms. For efficency, set it to 10 ms.", 1000 * this.inFrameSize / samplingRate);
            }
            logger.LogInfo("WebRTCAudioProcessor create sampling rate {0}, frame samples {1}", samplingRate, this.inFrameSize / this.channels);
        }
        bool aecInited;
        private void InitReverseStream()
        {
            lock (this)
            {
                if (!aecInited)
                {
                    if (disposed)
                    {
                        return;
                    }
                    int size = processFrameSize * reverseSamplingRate / samplingRate * reverseChannels;
                    reverseFramer = new Framer<float>(size);
                    reverseBuf = new short[processFrameSize * reverseChannels / channels]; // should match direct stream
                    if (reverseSamplingRate != samplingRate)
                    {
                        logger.LogWarning("WebRTCAudioProcessor AEC: output sampling rate {0} != {1} capture sampling rate. For better AEC, set audio source (microphone) and audio output samping rates to the same value.", reverseSamplingRate, samplingRate);
                    }
                    aecInited = true;
                }
            }
        }
        public short[] Process(short[] buf)
        {
            if (Bypass) return buf;
            if (disposed) return buf;
            if (proc == IntPtr.Zero) return buf;
            if (buf.Length != this.inFrameSize)
            {
                this.logger.LogError("WebRTCAudioProcessor Process: frame size expected: {0}, passed: {1}", this.inFrameSize, buf);
                return buf;
            }
            bool voiceDetected = false;
            for (int offset = 0; offset < inFrameSize; offset += processFrameSize)
            {
                bool vd = true;
                int err = webrtc_audio_processor_process(proc, buf, offset , out vd);
                if (vd)
                    voiceDetected = true;
                if (lastProcessErr != err)
                {
                    lastProcessErr = err;
                    this.logger.LogError("WebRTCAudioProcessor Process: webrtc_audio_processor_process() error {0}", err);
                    return buf;
                }
            }
            if (vad && !voiceDetected)
            {
                return null;
            }
            else
            {
                return buf;
            }
            
        }
        int lastProcessErr = 0;
        int lastProcessReverseErr = 0;
        
        public void OnAudioOutFrameFloat(float[] data)
        {
            if (disposed) return;
            if (proc == IntPtr.Zero) return;
            foreach (var reverseBufFloat in reverseFramer.Frame(data))
            {
                if (reverseBufFloat.Length != reverseBuf.Length)
                {
                    AudioUtil.ResampleAndConvert(reverseBufFloat, reverseBuf, reverseBuf.Length, this.reverseChannels);
                }
                else
                {
                    AudioUtil.Convert(reverseBufFloat, reverseBuf, reverseBuf.Length);
                }
                int err = webrtc_audio_processor_process_reverse(proc, reverseBuf, reverseBuf.Length);
                if (lastProcessReverseErr != err)
                {
                    lastProcessReverseErr = err;
                    this.logger.LogError("WebRTCAudioProcessor OnAudioOutFrameFloat: webrtc_audio_processor_process_reverse() error {0}", err);
                }
            }
        }
        private int setParam(int param, int v)
        {
            return webrtc_audio_processor_set_param(proc, param, v);
        }
        private int setConfigParam(int param, int v)
        {
            return webrtc_audio_processor_set_config_param(proc, param, v);
        }
        public void Dispose()
        {
            lock (this)
            {
                if (!disposed)
                {
                    disposed = true;
                    if (proc != IntPtr.Zero)
                    {
                        webrtc_audio_processor_destroy(proc);
                    }
                }
            }
        }
    }
    public class WebRTCAudioLib
    {
#if DLL_IMPORT_INTERNAL
        const string lib_name = "__Internal";
#else
        const string lib_name = "webrtc-audio";
#endif
        
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr webrtc_audio_processor_create(int samplingRate, int channels, int frameSize, int revSamplingRate, int revChannels);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_set_config_param(IntPtr proc, int param, int v);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_init(IntPtr proc);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_set_param(IntPtr proc, int param, int v);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_process(IntPtr proc, short[] buffer, int offset, out bool voiceDetected);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int webrtc_audio_processor_process_reverse(IntPtr proc, short[] buffer, int bufferSize);
        [DllImport(lib_name, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void webrtc_audio_processor_destroy(IntPtr proc);
        // library methods return webrtc error codes
        enum Error
        {
            // Fatal errors.
            kNoError = 0,
            kUnspecifiedError = -1,
            kCreationFailedError = -2,
            kUnsupportedComponentError = -3,
            kUnsupportedFunctionError = -4,
            kNullPointerError = -5,
            kBadParameterError = -6,
            kBadSampleRateError = -7,
            kBadDataLengthError = -8,
            kBadNumberChannelsError = -9,
            kFileError = -10,
            kStreamParameterNotSetError = -11,
            kNotEnabledError = -12,
            // Warnings are non-fatal.
            // This results when a set_stream_ parameter is out of range. Processing
            // will continue, but the parameter may have been truncated.
            kBadStreamParameterWarning = -13
        };
        enum AECMobileRoutingMode
        {
            kQuietEarpieceOrHeadset,
            kEarpiece,
            kLoudEarpiece,
            kSpeakerphone,
            kLoudSpeakerphone
        };
        public struct ConfigParam
        {
            public const int AEC_DELAY_AGNOSTIC = 12;
            public const int AEC_EXTENDED_FILTER = 13;
            public const int AGC_EXPERIMENTAL = 53;
            public const int AGC_EXPERIMENTAL_STARTUP_MIN_VOLUME = 54;
            public const int AGC_EXPERIMENTAL_CLIP_LEVEL_MIN = 55;
        };
        public struct Param
        {
            public const int REVERSE_STREAM_DELAY_MS = 1;
            public const int AEC = 10;
            public const int AEC_SUPPRESSION_LEVEL = 11;
            public const int AECM = 20;
            public const int AECM_ROUTING_MODE = 21;
            public const int AECM_COMFORT_NOISE = 22;
            public const int HIGH_PASS_FILTER = 31;
            public const int NS = 41;
            public const int NS_LEVEL = 42;
            public const int AGC = 51;
            public const int AGC_MODE = 52;
            public const int AGC_COMPRESSION_GAIN = 56;
            public const int AGC_LIMITER = 57;
            public const int VAD = 61;
            public const int VAD_FRAME_SIZE_MS = 62;
            public const int VAD_LIKELIHOOD = 63;
        }
    }
}
