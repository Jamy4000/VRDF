using UnityEngine;
using System.Collections.Generic;

namespace Photon.Voice.Unity
{
    // Plays back input audio via Unity AudioSource
    // May consume audio packets in thread other than Unity's main thread
    // TODO: When used w/o AudioStreamPlayer (if it makes sense), pause playback if no input audio to avoid looping sound.
    public class UnityAudioOut : ISyncAudioOut<float>
    {
        private int bufferSamples;
        private int inputSamplePos; 

        private AudioSource source;
        private bool started;

        public UnityAudioOut(AudioSource audioSource)
        {
            this.source = audioSource;
        }
        public int Lag { get { return 0; } }

        // non-wrapped play position
        public int PlaySamplePos
        {
            get { return this.started ? this.playLoopCount * this.bufferSamples + this.source.timeSamples : 0; }
            set
            {
                if (this.started)
                {
                    // if negative value is passed (possible when playback starts?), loop count is < 0 and sample position is positive
                    var pos = value % this.bufferSamples;
                    if (pos < 0)
                    {
                        pos += this.bufferSamples;
                    }
                    this.source.timeSamples = pos;
                    this.playLoopCount = value / this.bufferSamples;
                    this.sourceTimeSamplesPrev = this.source.timeSamples;
                }

            }
        }
        private int sourceTimeSamplesPrev;
        private int playLoopCount;

        public bool IsPlaying
        {
            get { return this.source.isPlaying; }
        }

        public void Start(int frequency, int channels, int frameSamples, int playDelayMs)
        {
            this.bufferSamples = playDelayMs * frequency / 1000 + frameSamples + frequency; // max delay + frame +  1 sec. just in case

            this.source.loop = true;
            // using streaming clip leads to too long delays
            this.source.clip = AudioClip.Create("AudioStreamPlayer", bufferSamples, channels, frequency, false);
            this.started = true;

            this.inputSamplePos = 0;
            this.PlaySamplePos = 0;

            this.source.Play();
            //        this.source.Pause();
        }

        Queue<float[]> frameQueue = new Queue<float[]>();
        public const int FRAME_POOL_CAPACITY = 50;
        PrimitiveArrayPool<float> framePool = new PrimitiveArrayPool<float>(FRAME_POOL_CAPACITY, "UnityAudioOut");

        // should be called in Update thread
        public void Service()
        {
            if (this.started)
            {
                lock (frameQueue)
                {
                    while (frameQueue.Count > 0)
                    {
                        var frame = frameQueue.Dequeue();
                        this.source.clip.SetData(frame, this.inputSamplePos % this.bufferSamples);
                        this.inputSamplePos += frame.Length / this.source.clip.channels;
                        framePool.Release(frame);
                    }
                }


                // loop detection (pcmsetpositioncallback not called when clip loops)
                if (this.source.isPlaying)
                {
                    if (this.source.timeSamples < sourceTimeSamplesPrev)
                    {
                        playLoopCount++;
                    }
                    sourceTimeSamplesPrev = this.source.timeSamples;
                }
            }
        }

        // may be called on any thread
        public void Push(float[] frame)
        {
            if (frame.Length == 0)
            {
                return;
            }

			//TODO: call framePool.AcquireOrCreate(frame.Length) and test
            if (framePool.Info != frame.Length)
            {
                framePool.Init(frame.Length);
            }
            float[] b = framePool.AcquireOrCreate();
            System.Buffer.BlockCopy(frame, 0, b, 0, frame.Length * sizeof(float));
            lock (frameQueue)
            {
                frameQueue.Enqueue(b);
            }
        }

        public void Stop()
        {
            this.started = false;
            if (this.source != null)
            {
                this.source.clip = null;
            }
        }

        public void Pause()
        {
            this.source.Pause();
        }

        public void UnPause()
        {
            this.source.UnPause();
        }
    }
}