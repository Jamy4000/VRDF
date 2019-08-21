// ----------------------------------------------------------------------------
// <copyright file="Speaker.cs" company="Exit Games GmbH">
//   Photon Voice for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
// Component representing remote audio stream in local scene.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------


using System;
using UnityEngine;


namespace Photon.Voice.Unity
{
    /// <summary> Component representing remote audio stream in local scene. </summary>
    [RequireComponent(typeof(AudioSource))]
    [AddComponentMenu("Photon Voice/Speaker")]
    public class Speaker : VoiceComponent
    {
        #region Private Fields

        private IAudioOut<float> audioOutput;

        private RemoteVoiceLink remoteVoiceLink;

        private bool started;

        #endregion

        #region Public Fields

        ///<summary>Remote audio stream playback delay to compensate packets latency variations. Try 100 - 200 if sound is choppy.</summary> 
        public int PlayDelayMs = 200;

        #if UNITY_PS4
        /// <summary>Set the PS4 User ID to determine on which controller to play audio.</summary> 
        /// <remarks>
        /// Note: at the moment, only the first Speaker can successfully set the User ID. 
        /// Subsequently initialized Speakers will play their audio on the controller set with the first Speaker initialized.
        /// </remarks>
        public int PS4UserID = 0;
        #endif

        #endregion

        #region Properties

        /// <summary>Is the speaker playing right now.</summary>
        public bool IsPlaying
        {
            get { return this.audioOutput != null && this.audioOutput.IsPlaying; }
        }

        /// <summary>Smoothed difference between (jittering) stream and (clock-driven) audioOutput.</summary>
        public int Lag
        {
            get { return this.audioOutput != null ? this.audioOutput.Lag : -1; }
        }

        /// <summary>
        /// Register a method to be called when remote voice removed.
        /// </summary>
        public Action<Speaker> OnRemoteVoiceRemoveAction { get; set; }

        /// <summary>Per room, the connected users/players are represented with a Realtime.Player, also known as Actor.</summary>
        /// <remarks>Photon Voice calls this Actor, to avoid a name-clash with the Player class in Voice.</remarks>
        public Realtime.Player Actor { get; protected internal set; }

        /// <summary>
        /// Whether or not this Speaker has been linked to a remote voice stream.
        /// </summary>
        public bool IsLinked
        {
            get { return this.remoteVoiceLink != null; }
        }

        #if UNITY_EDITOR
        public RemoteVoiceLink RemoteVoiceLink
        {
            get { return this.remoteVoiceLink; }
        }
        #endif

        #endregion

        #region Private Methods

        protected override void Awake()
        {
            base.Awake();
            Func<IAudioOut<float>> factory = () => new AudioStreamPlayer<float>(new VoiceLogger(this, "AudioStreamPlayer", this.LogLevel),  
                new UnityAudioOut(this.GetComponent<AudioSource>()), "PhotonVoiceSpeaker:", this.Logger.IsInfoEnabled);

            #if !UNITY_EDITOR && UNITY_PS4
            this.audioOutput = new Photon.Voice.PS4.PS4AudioOut(PS4UserID, factory);
            #else
            this.audioOutput = factory();
            #endif
            this.StartPlaying();
        }

        internal void OnRemoteVoiceInfo(RemoteVoiceLink stream)
        {
            if (stream == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("RemoteVoiceLink is null, cancelled linking");
                }
                return;
            }
            if (this.IsLinked)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Speaker already linked to {0}/{1}, cancelled linking to {2}/{3}",
                        this.remoteVoiceLink.PlayerId, this.remoteVoiceLink.VoiceId, stream.PlayerId, stream.VoiceId);
                }
                return;
            }
            if (stream.Info.Channels <= 0) // early avoid possible crash due to ArgumentException in AudioClip.Create inside UnityAudioOut.Start
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Received voice info channels is not expected: {0} <= 0, cancelled linking to {1}/{2}", stream.Info.Channels, 
                        stream.PlayerId, stream.VoiceId);
                }
                return;
            }
            this.remoteVoiceLink = stream;
            this.remoteVoiceLink.RemoteVoiceRemoved += this.OnRemoteVoiceRemove;
            this.StartPlaying();
        }

        internal void OnRemoteVoiceRemove()
        {
            bool wasStarted = this.started;
            if (this.audioOutput != null)
            {
                this.audioOutput.Stop();
                this.started = false;
            }
            this.Actor = null;
            if (this.OnRemoteVoiceRemoveAction != null) { this.OnRemoteVoiceRemoveAction(this); }
            if (this.remoteVoiceLink != null)
            {
                this.remoteVoiceLink.RemoteVoiceRemoved -= this.OnRemoteVoiceRemove;
                if (wasStarted)
                {
                    this.remoteVoiceLink.FloatFrameDecoded -= this.OnAudioFrame;
                }
                this.remoteVoiceLink = null;
            }
        }

        internal void OnAudioFrame(float[] frame)
        {
            this.audioOutput.Push(frame);
        }

        private void Update()
        {
            this.audioOutput.Service();
        }

        private void StartPlaying()
        {
            if (!this.started && this.audioOutput != null && this.IsLinked)
            {
                VoiceInfo voiceInfo = this.remoteVoiceLink.Info;
                this.audioOutput.Start(voiceInfo.SamplingRate, voiceInfo.Channels, voiceInfo.FrameDurationSamples, this.PlayDelayMs);
                this.remoteVoiceLink.FloatFrameDecoded += this.OnAudioFrame;
                this.started = true;
            }
        }

        #endregion
    }
}