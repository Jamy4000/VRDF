using System;
using UnityEngine;

namespace Photon.Voice.Unity
{
    // depends on Unity's AndroidJavaProxy
    public class UnityAndroidAudioInAEC : Voice.IAudioPusher<short>
    {
        class DataCallback : AndroidJavaProxy
        {
            Action<short[]> callback;
            IntPtr javaBuf;
            public DataCallback() : base("com.exitgames.photon.audioinaec.AudioInAEC$DataCallback") { }
            public void SetCallback(Action<short[]> callback, IntPtr javaBuf)
            {
                this.callback = callback;
                this.javaBuf = javaBuf;
            }
            public void OnData()
            {
                if (callback != null)
                {
                    //TODO: copy to LocalVoiceFramed.PushDataBufferPool element instead
                    var buf = AndroidJNI.FromShortArray(javaBuf);
                    cntFrame++;
                    cntShort += buf.Length;
                    this.callback(buf);
                }
            }
            public void OnStop()
            {
                AndroidJNI.DeleteGlobalRef(javaBuf);
            }
            int cntFrame;
            int cntShort;

        }

        AndroidJavaObject audioIn;
        IntPtr javaBuf;
        Voice.ILogger logger;
        public UnityAndroidAudioInAEC(Voice.ILogger logger)
        {
            this.logger = logger;
            try
            {
                this.callback = new DataCallback();
                audioIn = new AndroidJavaObject("com.exitgames.photon.audioinaec.AudioInAEC");
                bool aecAvailable = audioIn.Call<bool>("AECIsAvailable");
                int minBufSize = audioIn.Call<int>("GetMinBufferSize", SamplingRate, Channels);
                logger.LogInfo("[PV] UnityAndroidAudioInAEC: AndroidJavaObject created: aecAvailable: {0}, minBufSize: {1}", aecAvailable, minBufSize);

                AndroidJavaClass app = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject activity = app.GetStatic<AndroidJavaObject>("currentActivity");
                // Set buffer IntPtr reference separately via pure jni call, pass other values and start capture via AndroidJavaObject helper

                var ok = audioIn.Call<bool>("Start", activity, this.callback, SamplingRate, Channels, minBufSize * 4, aecAvailable);
                if (ok)
                {
                    logger.LogInfo("[PV] UnityAndroidAudioInAEC: AndroidJavaObject started: {0}, sampling rate: {1}, channels: {2}, record buffer size: {3}, aec: {4}", ok, SamplingRate, Channels, minBufSize * 4, aecAvailable);
                }
                else
                {
                    Error = "[PV] UnityAndroidAudioInAEC constructor: calling Start java method failure";
                    logger.LogError("[PV] UnityAndroidAudioInAEC: {0}", Error);
                }                
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in WindowsAudioInPusher constructor";
                }
                logger.LogError("[PV] UnityAndroidAudioInAEC: {0}", Error);
            }
        }

        // Supposed to be called once at voice initialization.
        // Otherwise recreate native object (instead of adding 'set callback' method to java interface)
        public void SetCallback(Action<short[]> callback, ObjectFactory<short[], int> bufferFactory)
        {
            if (Error == null)
            {
                var voiceFrameSize = bufferFactory.Info;
                // setting to voice FrameSize lets to avoid framing procedure
                javaBuf = AndroidJNI.NewGlobalRef(AndroidJNI.NewShortArray(voiceFrameSize));
                this.callback.SetCallback(callback, javaBuf);
                var meth = AndroidJNI.GetMethodID(audioIn.GetRawClass(), "SetBuffer", "([S)Z");
                bool ok = AndroidJNI.CallBooleanMethod(audioIn.GetRawObject(), meth, new jvalue[] { new jvalue() { l = javaBuf } });
                if (!ok)
                {
                    Error = "UnityAndroidAudioInAEC.SetCallback(): calling SetBuffer java method failure";
                }
            }
            if (Error != null)
            {
                logger.LogError("[PV] UnityAndroidAudioInAEC: {0}", Error);
            }
        }

        DataCallback callback;

        public int Channels { get { return 1; } }

        public int SamplingRate { get { return 44100; } }

        public string Error { get; private set; }

        public void Dispose()
        {
            if (audioIn != null)
            {
                audioIn.Call<bool>("Stop");
            }
        }
    }
}