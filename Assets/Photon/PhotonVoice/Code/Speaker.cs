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

        private bool initialized; // awake called

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
        /// <summary>
        /// USE IN EDITOR ONLY
        /// </summary>
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
            this.initialized = true;
            if (this.IsLinked)
            {
                this.StartPlaying();
            }
        }

        internal bool OnRemoteVoiceInfo(RemoteVoiceLink stream)
        {
            if (stream == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("RemoteVoiceLink is null, cancelled linking");
                }
                return false;
            }
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnRemoteVoiceInfo {0}/{1}", stream.PlayerId, stream.PlayerId);
            }
            if (this.IsLinked)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Speaker already linked to {0}/{1}, cancelled linking to {2}/{3}",
                        this.remoteVoiceLink.PlayerId, this.remoteVoiceLink.VoiceId, stream.PlayerId, stream.VoiceId);
                }
                return false;
            }
            if (stream.Info.Channels <= 0) // early avoid possible crash due to ArgumentException in AudioClip.Create inside UnityAudioOut.Start
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Received voice info channels is not expected: {0} <= 0, cancelled linking to {1}/{2}", stream.Info.Channels, 
                        stream.PlayerId, stream.VoiceId);
                }
                return false;
            }
            this.remoteVoiceLink = stream;
            this.remoteVoiceLink.RemoteVoiceRemoved += this.OnRemoteVoiceRemove;
            return !this.initialized || this.StartPlaying();
        }

        internal void OnRemoteVoiceRemove()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnRemoteVoiceRemove {0}/{1}", this.remoteVoiceLink.PlayerId, this.remoteVoiceLink.PlayerId);
            }
            this.StopPlaying();
            if (this.OnRemoteVoiceRemoveAction != null) { this.OnRemoteVoiceRemoveAction(this); }
            this.CleanUp();
        }

        internal void OnAudioFrame(float[] frame)
        {
            this.audioOutput.Push(frame);
        }

        private void Update()
        {
            this.audioOutput.Service();
        }
        
        private bool StartPlaying()
        {
            if (!this.IsLinked)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot start playback because speaker is not linked");
                }
                return false;
            }
            if (this.started)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Playback is already started");
                }
                return false;
            }
            if (!this.initialized)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot start playback because not initialized yet");
                }
                return false;
            }
            if (this.audioOutput == null)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogWarning("Cannot start playback because audioOutput is null");
                }
                return false;
            }
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("StartPlaying {0}/{1}", this.remoteVoiceLink.PlayerId, this.remoteVoiceLink.PlayerId);
            }
            VoiceInfo voiceInfo = this.remoteVoiceLink.Info;
            if (voiceInfo.Channels == 0)
            {
                if (this.Logger.IsErrorEnabled)
                {
                    this.Logger.LogError("Cannot start playback because remoteVoiceLink.Info.Channels == 0");
                }
                return false;
            } 
            this.audioOutput.Start(voiceInfo.SamplingRate, voiceInfo.Channels, voiceInfo.FrameDurationSamples, this.PlayDelayMs);
            this.remoteVoiceLink.FloatFrameDecoded += this.OnAudioFrame;
            this.started = true;
            return true;
        }

        private void OnDestroy()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("OnDestroy");
            }
            if (this.started)
            {
                this.StopPlaying();
            }
            this.CleanUp();
        }
        
        private bool StopPlaying()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("StopPlaying");
            }
            if (!this.started)
            {
                if (this.Logger.IsWarningEnabled)
                {
                    this.Logger.LogWarning("Cannot stop playback because it's not started");
                }
                return false;
            }
            if (this.IsLinked)
            {
                this.remoteVoiceLink.FloatFrameDecoded -= this.OnAudioFrame;
            }
            else if (this.Logger.IsWarningEnabled)
            {
                this.Logger.LogWarning("Speaker not linked while stopping playback");
            }
            if (this.audioOutput != null)
            {
                this.audioOutput.Stop();
            }
            else if (this.Logger.IsWarningEnabled)
            {
                this.Logger.LogWarning("audioOutput is null while stopping playback");
            }
            this.started = false;
            return true;
        }

        private void CleanUp()
        {
            if (this.Logger.IsDebugEnabled)
            {
                this.Logger.LogDebug("CleanUp");
            }
            if (this.remoteVoiceLink != null)
            {
                this.remoteVoiceLink.RemoteVoiceRemoved -= this.OnRemoteVoiceRemove;
                this.remoteVoiceLink = null;
            }
            this.Actor = null;
        }

        #endregion
    }
}