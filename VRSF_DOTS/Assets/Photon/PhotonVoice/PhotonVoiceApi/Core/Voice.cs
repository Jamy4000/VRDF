// -----------------------------------------------------------------------
// <copyright file="Voice.cs" company="Exit Games GmbH">
//   Photon Voice API Framework for Photon - Copyright (C) 2017 Exit Games GmbH
// </copyright>
// <summary>
//   Photon data streaming support.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Threading;
#if NETFX_CORE
using Windows.System.Threading;
#endif
namespace Photon.Voice
{
    /// <summary>
    /// Interface for pulling data, in case this is more appropriate than pushing it.
    /// </summary>
    public interface IDataReader<T> : IDisposable
    {
        /// <summary>Fill full given frame buffer with source uncompressed data or return false if not enough such data.</summary>
        /// <param name="buffer">Buffer to fill.</param>
        /// <returns>True if buffer was filled successfully, false otherwise.</returns>
        bool Read(T[] buffer);
    }
    /// <summary>
    /// Interface for classes that want their Service() function to be called regularly in the context of a LocalVoice.
    /// </summary>
    public interface IServiceable
    {
        /// <summary>Service function that should be called regularly.</summary>
        void Service(LocalVoice localVoice);
    }
    /// <summary>
    /// Represents outgoing data stream.
    /// </summary>
    public class LocalVoice : IDisposable
    {
        public const int DATA_POOL_CAPACITY = 50; // TODO: may depend on data type and properties, set for average audio stream
        [Obsolete("Use InterestGroup.")]
        public byte Group { get { return InterestGroup; } set { InterestGroup = value; } }
        /// <summary>If InterestGroup != 0, voice's data is sent only to clients listening to this group (if supported by transport).</summary>
        public byte InterestGroup { get; set; }
        /// <summary>Returns Info structure assigned on local voice cration.</summary>
        public VoiceInfo Info { get { return info; } }
        /// <summary>If true, stream data broadcasted.</summary>
        public bool TransmitEnabled { get; set; }
        /// <summary>Returns true if stream broadcasts.</summary>
        public bool IsCurrentlyTransmitting { get; protected set; }
        /// <summary>Sent frames counter.</summary>
        public int FramesSent { get; private set; }
        /// <summary>Sent frames bytes counter.</summary>
        public int FramesSentBytes { get; private set; }
        /// <summary>Send data reliable.</summary>
        public bool Reliable { get; set; }
        /// <summary>Send data encrypted.</summary>
        public bool Encrypt { get; set; }
        /// <summary>Optional user object attached to LocalVoice. its Service() will be called at each VoiceClient.Service() call.</summary>
        public IServiceable LocalUserServiceable { get; set; }
        virtual public IEncoder CreateDefaultEncoder(VoiceInfo info)
        {
            throw new UnsupportedCodecException("LocalVoice.CreateDefaultEncoder", info.Codec, Logger);
        }
        /// <summary>
        /// If true, outgoing stream routed back to client via server same way as for remote client's streams.
        /// Can be swithed any time. OnRemoteVoiceInfoAction and OnRemoteVoiceRemoveAction are triggered if required.
        /// This functionality availability depends on transport.
        /// </summary>
        public bool DebugEchoMode
        {
            get { return debugEchoMode; }
            set
            {
                if (debugEchoMode != value)
                {
                    debugEchoMode = value;
                    if (voiceClient != null && voiceClient.transport != null)
                    {
                        voiceClient.transport.SetDebugEchoMode(this);
                    }
                }
            }
        }
        bool debugEchoMode;
        internal ILogger Logger { get { return voiceClient.transport; } }
#region nonpublic
        internal VoiceInfo info;
        protected IEncoder encoder;
        internal byte id;
        internal int channelId;
        internal byte evNumber = 0; // sequence used by receivers to detect loss. will overflow.
        protected VoiceClient voiceClient;
        volatile protected bool disposed;
        protected object disposeLock = new object();
        internal LocalVoice() // for dummy voices
        {
        }
        internal LocalVoice(VoiceClient voiceClient, IEncoder encoder, byte id, VoiceInfo voiceInfo, int channelId)
        {            
            this.TransmitEnabled = true;
            this.info = voiceInfo;
            this.channelId = channelId;
            this.voiceClient = voiceClient;
            this.id = id;
            if (encoder == null)
            {
                voiceClient.transport.LogInfo(LogPrefix + ": Creating default encoder");
                this.encoder = CreateDefaultEncoder(voiceInfo);
            }
            else
            {
                this.encoder = encoder;
            }
            this.encoder.Output = sendFrame;
        }
        internal string Name { get { return "Local " + info.Codec + " v#" + id + " ch#" + voiceClient.channelStr(channelId); } }
        internal string LogPrefix { get { return "[PV] " + Name; } }
        
        private int noTransmitCnt;
        protected void resetNoTransmitCnt()
        {
            noTransmitCnt = 10; // timeout * service() calls frequency
        }
        internal virtual void service()
        {
            if (this.voiceClient.transport.IsChannelJoined(this.channelId) && this.TransmitEnabled)
            {
                while (true)
                {
                    var x = encoder.DequeueOutput();
                    if (x.Count == 0)
                    {
                        break;
                    }
                    else
                    {
                        sendFrame(x);
                    }
                }
            }
            if (noTransmitCnt == 0)
            {
                this.IsCurrentlyTransmitting = false;
            }
            else
            {
                this.IsCurrentlyTransmitting = true;
                noTransmitCnt--;
            }
            if (LocalUserServiceable != null)
            {
                LocalUserServiceable.Service(this);
            }
        }
        internal void sendFrame(ArraySegment<byte> compressed)
        {
            this.FramesSent++;
            this.FramesSentBytes += compressed.Count;
            this.voiceClient.transport.SendFrame(compressed, evNumber, id, this.channelId, this);
            this.eventTimestamps[evNumber] = Environment.TickCount;
            evNumber++;
            resetNoTransmitCnt();
        }
        internal Dictionary<byte, int> eventTimestamps = new Dictionary<byte, int>();
#endregion
        /// <summary>Remove this voice from it's VoiceClient (using VoiceClient.RemoveLocalVoice</summary>.</summary>
        public void RemoveSelf()
        {
            if (this.voiceClient != null) // dummy voice can try to remove self
			{
	            this.voiceClient.RemoveLocalVoice(this);
			}
        }
        public virtual void Dispose()
        {
            if (!disposed)
            {
                if (this.encoder != null)
                {
                    this.encoder.Dispose();
                }
                disposed = true;
            }
        }
    }
    /// <summary>Event Actions and other options for a remote voice (incoming stream).</summary>
    public struct RemoteVoiceOptions
    {
        /// <summary>
        /// Register a method to be called when new data frame received..
        /// </summary>
        public void SetOutput(Action<float[]> output)
        {
            outType = OutputType.Float;
            this.output = output;
        }
        public void SetOutput(Action<short[]> output)
        {
            outType = OutputType.Short;
            this.output = output;
        }
        public void SetOutput(Action<ImageOutputBuf> output)
        {
            outType = OutputType.Image;
            this.output = output;
        }
        /// <summary>
        /// Register a method to be called when the remote voice is removed.
        /// </summary>
        public Action OnRemoteVoiceRemoveAction { get; set; }
        
        /// <summary>Remote voice data decoder. Use to set decoder options or override it with user decoder.</summary>
        public IDecoder Decoder { get; set; }
        public ImageFormat OutputImageFormat { get; set; }
        public Flip OutputImageFlip { get; set; }
        internal enum OutputType { None, Float, Short, Image};
        internal OutputType outType { get; private set; }
        internal Object output { get; private set; }
}
    internal class RemoteVoice : IDisposable
    {
        // Client.RemoteVoiceInfos support
        internal VoiceInfo Info { get; private set; }
        internal RemoteVoiceOptions options;
        internal int channelId;
        private int playerId;
        private byte voiceId;
        volatile private bool disposed;
        object disposeLock = new object();
        internal RemoteVoice(VoiceClient client, RemoteVoiceOptions options, int channelId, int playerId, byte voiceId, VoiceInfo info, byte lastEventNumber)
        {            
            this.options = options;
            this.voiceClient = client;
            this.channelId = channelId;
            this.playerId = playerId;
            this.voiceId = voiceId;
            this.Info = info;
            this.lastEvNumber = lastEventNumber;
#if NETFX_CORE
            ThreadPool.RunAsync((x) =>
            {
                decodeThread();
            });
#else
            var t = new Thread(() => decodeThread());
            t.Name = LogPrefix + " decode";
            t.Start();
#endif
        }
        protected string Name { get { return "Remote " + Info.Codec +  " v#" + voiceId + " ch#" + voiceClient.channelStr(channelId) + " p#" + playerId; } }
        protected string LogPrefix { get { return "[PV] " + Name; } }
        internal byte lastEvNumber = 0;
        private VoiceClient voiceClient;
        private static byte byteDiff(byte latest, byte last)
        {
            return (byte)(latest - (last + 1));
        }
        internal void receiveBytes(byte[] receivedBytes, byte evNumber)
        {
            // receive-gap detection and compensation
            if (evNumber != this.lastEvNumber) // skip check for 1st event 
            {
                int missing = byteDiff(evNumber, this.lastEvNumber);
                if (missing == 0)
                {
                    this.lastEvNumber = evNumber;
                }
                else if (missing < 127)
                {
                    this.voiceClient.transport.LogWarning(LogPrefix + " evNumer: " + evNumber + " playerVoice.lastEvNumber: " + this.lastEvNumber + " missing: " + missing + " r/b " + receivedBytes.Length);
                    this.voiceClient.FramesLost += missing;
                    this.lastEvNumber = evNumber;
                    // restoring missing frames
                    receiveNullFrames(missing);
                } else {
                    // late (out of order) frames, just ignore them
                    // these frames already counted in FramesLost
                    this.voiceClient.transport.LogWarning(LogPrefix + " evNumer: " + evNumber + " playerVoice.lastEvNumber: " + this.lastEvNumber + " late: " + (255 - missing) + " r/b " + receivedBytes.Length);
                }
            }
            this.receiveFrame(receivedBytes);
        }
        Queue<byte[]> frameQueue = new Queue<byte[]>();
        AutoResetEvent frameQueueReady = new AutoResetEvent(false);
        void receiveFrame(byte[] frame)
        {
            lock (disposeLock) // sync with Dispose and decodeThread 'finally'
            {
                if (disposed) return;
                lock (frameQueue)
                {
                    frameQueue.Enqueue(frame);
                }
                frameQueueReady.Set();
            }
        }
        void receiveNullFrames(int count)
        {
            lock (disposeLock) // sync with Dispose and decodeThread 'finally'
            {
                if (disposed) return;
                lock (frameQueue)
                {
                    for (int i = 0; i < count; i++)
                    {
                        frameQueue.Enqueue(null);
                    }
                }
                frameQueueReady.Set();
            }
        }
        IDecoder createDefaultDecoder()
        {
            switch (options.outType)
            {
                case RemoteVoiceOptions.OutputType.Float:
                    if (Info.Codec == Codec.AudioOpus)
                    {
                        voiceClient.transport.LogInfo(LogPrefix + ": Creating default decoder for output type = " + options.outType);
                        return new OpusCodec.DecoderFloat(options.output as Action<float[]>, voiceClient.transport);
                    }
                    else
                    {
                        voiceClient.transport.LogError(LogPrefix + ": Action<float[]> output set for not audio decoder (output type = " + options.outType + ")");
                        return null;
                    }
                case RemoteVoiceOptions.OutputType.Short:
                    if (Info.Codec == Codec.AudioOpus)
                    {
                        voiceClient.transport.LogInfo(LogPrefix + ": Creating default decoder for output type = " + options.outType);
                        return new OpusCodec.DecoderShort(options.output as Action<short[]>, voiceClient.transport);
                    }
                    else
                    {
                        voiceClient.transport.LogError(LogPrefix + ": Action<short[]> output set for not audio decoder (output type = " + options.outType + ")");
                        return null;
                    }
#if PHOTON_VOICE_VIDEO_ENABLE
                case RemoteVoiceOptions.OutputType.Image:
                    voiceClient.transport.LogInfo(LogPrefix + ": Creating default decoder for output type = " + options.outType);
                    IDecoderQueuedOutputImageNative vd;
                    switch (Info.Codec)
                    {
                        case Codec.VideoVP8:
                        case Codec.VideoVP9:
                            vd = new VPxCodec.Decoder(options.output as Action<ImageOutputBuf>, voiceClient.transport);
                            break;
                        case Codec.VideoH264:
                            vd = new FFmpegCodec.Decoder(options.output as Action<ImageOutputBuf>, voiceClient.transport);
                            break;
                        default:
                            voiceClient.transport.LogError(LogPrefix + ": Action<ImageOutputBuf> output set for not video decoder (output type = " + options.outType + ")");
                            return null;
                    }
                    if (options.OutputImageFormat != ImageFormat.Undefined)
                    {
                        vd.OutputImageFormat = options.OutputImageFormat;
                    }
                    if (options.OutputImageFlip != Flip.Undefined)
                    {
                        vd.OutputImageFlip = options.OutputImageFlip;
                    }
                    return vd;
#endif
                case RemoteVoiceOptions.OutputType.None:
                default:
                    voiceClient.transport.LogError(LogPrefix + ": Output must be set in RemoteVoiceOptions with SetOutput call (output type = " + options.outType + ")");
                    return null;
            }
        }
        void decodeThread()
        {
//#if UNITY_5_3_OR_NEWER
//            UnityEngine.Profiling.Profiler.BeginThreadProfiling("PhotonVoice", LogPrefix);
//#endif
            IDecoder decoder;
            if (this.options.Decoder == null)
            {
                decoder = createDefaultDecoder();                
            }
            else
            {
                if (options.outType == RemoteVoiceOptions.OutputType.None)
                {
                    decoder = this.options.Decoder;
                }
                else
                {
                    decoder = null;
                    voiceClient.transport.LogError(LogPrefix + ": Setting decoder output with RemoteVoiceOptions SetOutput is not supported for custom decoders (set via RemoteVoiceOptions Decoder property). Assign output directly to decoder (output type = " + options.outType + ")");
                }
            }
            if (decoder == null)
            {
                lock (disposeLock)
                {
                    disposed = true;
                }
                return;
            }
            voiceClient.transport.LogInfo(LogPrefix + ": Starting decode thread");
            try
            {
                decoder.Open(Info);
                while (!disposed)
                {
                    frameQueueReady.WaitOne(); // Wait until data is pushed to the queue or Dispose signals.
//#if UNITY_5_3_OR_NEWER
//                    UnityEngine.Profiling.Profiler.BeginSample("Decoder");
//#endif
                    while (true) // Dequeue and process while the queue is not empty
                    {
                        if (disposed) break; // early exit to save few resources
                        byte[] f = null;
                        bool ok = false;
                        lock (frameQueue)
                        {
                            if (frameQueue.Count > 0)
                            {
                                ok = true;
                                f = frameQueue.Dequeue();
                            }
                        }
                        if (ok)
                        {
                            decoder.Input(f);
                        }
                        else
                        {
                            break;
                        }
                    }
//#if UNITY_5_3_OR_NEWER
//                    UnityEngine.Profiling.Profiler.EndSample();
//#endif
                }
            }
            catch (Exception e)
            {
                voiceClient.transport.LogError(LogPrefix + ": Exception in decode thread: " + e);
                throw e;
            }
            finally
            {
                lock (disposeLock) // sync with receiveFrame/receiveNullFrames
                {
                    disposed = true; // set to disposing state if exiting due to exception
                }
                // cleaning up being sure that fields are not updated anymore
#if NETFX_CORE
                frameQueueReady.Dispose();
#else
                frameQueueReady.Close();
#endif
                lock (frameQueue)
                {
                    frameQueue.Clear();
                }
                decoder.Dispose();
                voiceClient.transport.LogInfo(LogPrefix + ": Exiting decode thread");
//#if UNITY_5_3_OR_NEWER
//                UnityEngine.Profiling.Profiler.EndThreadProfiling();
//#endif
            }
        }
        internal void removeAndDispose()
        {
            if (options.OnRemoteVoiceRemoveAction != null)
            {
                options.OnRemoteVoiceRemoveAction();
            }
            Dispose();
        }
        public void Dispose()
        {
            lock (disposeLock) // sync with receiveFrame/receiveNullFrames
            {
                if (!disposed)
                {
                    disposed = true;
                    frameQueueReady.Set(); // let decodeThread dispose resporces and exit
                }
            }
        }
    }
}
