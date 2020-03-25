
namespace Photon.Voice
{
    // Synchronizes provided ISyncAudioOut instance playback position with input stream position by pausing and unpausing ISyncAudioOut instance
    public class AudioStreamPlayer<T> : IAudioOut<T>
    {
        // fast forward if we are more than this value before desired position (stream pos - playDelaySamples)
        const int maxPlayLagMs = 100;
        private int maxPlayLagSamples;
        // buffering by playing few samples back
        private int playDelaySamples;
        private int frameSize;
        private int frameSamples;
        private int streamSamplePos;
        ILogger logger;
        /// <summary>Smoothed difference between (jittering) stream and (clock-driven) player.</summary>        
        private int CurrentBufferLag; // public property turned to private field; upper camel case name preserved to minify diff
        // jitter-free stream position
        private int streamSamplePosAvg;
        private ISyncAudioOut<T> audioOut;
        private bool audioOutStarted;
        private string logPrefix;
        private bool debugInfo;
        public AudioStreamPlayer(ILogger logger, ISyncAudioOut<T> audioOut, string logPrefix, bool debugInfo)
        {
            this.logger = logger;
            this.audioOut = audioOut;
            this.logPrefix = logPrefix;
            this.debugInfo = debugInfo;
        }
        public int Lag { get { return CurrentBufferLag; } }
        public bool IsPlaying
        {
            get { return this.audioOut.IsPlaying; }
        }
        public void Start(int frequency, int channels, int frameSamples, int playDelayMs)
        {
            this.frameSamples = frameSamples;
            this.frameSize = frameSamples * channels;
            // add 1 frame samples to make sure that we have something to play when delay set to 0
            this.maxPlayLagSamples = maxPlayLagMs * frequency / 1000 + this.frameSamples;
            this.playDelaySamples = playDelayMs * frequency / 1000 + this.frameSamples;
            // init with target value
            this.CurrentBufferLag = this.playDelaySamples;
            this.streamSamplePosAvg = this.playDelaySamples;
            this.streamSamplePos = 0;
            this.audioOut.Start(frequency, channels, frameSamples, maxPlayLagMs + playDelayMs);
            this.audioOutStarted = true;
        }
        // should be called in Update thread
        public void Service()
        {
            if (this.audioOutStarted)
            {
                this.audioOut.Service();
                var playPos = this.audioOut.PlaySamplePos; // cache calculated value
                // average jittering value
                this.CurrentBufferLag = (this.CurrentBufferLag * 39 + (this.streamSamplePos - playPos)) / 40;
                // calc jitter-free stream position based on clock-driven player position and average lag
                this.streamSamplePosAvg = playPos + this.CurrentBufferLag;
                if (this.streamSamplePosAvg > this.streamSamplePos)
                {
                    this.streamSamplePosAvg = this.streamSamplePos;
                }
                //Debug.LogFormat("===== Service {0} {1} {2}", playPos, this.streamSamplePos, this.playDelaySamples);
                // start with given delay or when stream position is ok after overrun pause
                if (playPos < this.streamSamplePos - this.playDelaySamples)
                {
                    if (!this.audioOut.IsPlaying)
                    {
                        this.audioOut.UnPause();
                    }
                }
                if (playPos > this.streamSamplePos - frameSamples)
                {
                    if (this.audioOut.IsPlaying)
                    {
                        if (this.debugInfo)
                        {
                            logger.LogWarning("{0} player overrun: {1}/{2}({3}) = {4}", this.logPrefix, playPos, streamSamplePos, streamSamplePosAvg, streamSamplePos - playPos);
                        }
                        // when nothing to play:
                        // pause player  (useful in case if stream is stopped for good) ...
                        this.audioOut.Pause();
                        // ... and rewind to proper position 
                        playPos = this.streamSamplePos;
                        this.audioOut.PlaySamplePos = playPos;
                        this.CurrentBufferLag = this.playDelaySamples;
                    }
                }
                if (this.audioOut.IsPlaying)
                {
                    var lowerBound = this.streamSamplePos - this.playDelaySamples - maxPlayLagSamples;
                    if (playPos < lowerBound)
                    {
                        if (this.debugInfo)
                        {
                            logger.LogWarning("{0} player underrun: {1}/{2}({3}) = {4}", this.logPrefix, playPos, streamSamplePos, streamSamplePosAvg, streamSamplePos - playPos);
                        }
                        // if lag exceeds max allowable, fast forward to proper position                    
                        playPos = this.streamSamplePos - this.playDelaySamples;
                        this.audioOut.PlaySamplePos = playPos;
                        this.CurrentBufferLag = this.playDelaySamples;
                    }
                }
            }
        }
        // may be called on any thread
        public void Push(T[] frame)
        {
            if (frame.Length == 0)
            {
                return;
            }
            if (frame.Length != frameSize)
            {
                logger.LogError("{0} Audio frames are not of  size: {1} != {2}", this.logPrefix, frame.Length, frameSize);
                //Debug.LogErrorFormat("{0} {1} {2} {3} {4} {5} {6}", frame[0], frame[1], frame[2], frame[3], frame[4], frame[5], frame[6]);
                return;
            }
            // Store last packet
            if (this.audioOutStarted)
            {
                this.audioOut.Push(frame);
                this.streamSamplePos += this.frameSamples;
            }
        }
        public void Stop()
        {
            this.audioOut.Stop();
            this.audioOutStarted = false;
        }
    }
}
