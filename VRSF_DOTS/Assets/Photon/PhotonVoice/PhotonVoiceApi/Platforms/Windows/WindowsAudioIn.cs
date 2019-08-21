#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System;
using System.Runtime.InteropServices;

namespace Photon.Voice.Windows
{
    public class WindowsAudioInPusher : IAudioPusher<short>
    {
        enum SystemMode
        {
            SINGLE_CHANNEL_AEC = 0,
            OPTIBEAM_ARRAY_ONLY = 2,
            OPTIBEAM_ARRAY_AND_AEC = 4,
            SINGLE_CHANNEL_NSAGC = 5,
        }

        [DllImport("AudioIn")]
        private static extern IntPtr Photon_Audio_In_Create(SystemMode systemMode, int micDevIdx, int spkDevIdx, Action<IntPtr, int> callback, bool featrModeOn, bool noiseSup, bool agc, bool cntrClip);

        [DllImport("AudioIn")]
        private static extern void Photon_Audio_In_Destroy(IntPtr handler);

        IntPtr handle;
        Action<short[]> pushCallback;
        ObjectFactory<short[], int> bufferFactory;

        public WindowsAudioInPusher(int deviceID, ILogger logger)
        {
            pushRef = push;            
            try
            {
                // use default playback device
                handle = Photon_Audio_In_Create(SystemMode.SINGLE_CHANNEL_AEC, deviceID, -1, pushRef, true, true, true, true); // defaults in original ms sample: false, true, false, false
            }
            catch (Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in WindowsAudioInPusher constructor";
                }
                logger.LogError("[PV] WindowsAudioInPusher: " + Error);
            }
        }
        Action<IntPtr, int> pushRef;
        // Supposed to be called once at voice initialization.
        // Otherwise recreate native object (instead of adding 'set callback' method to native interface)
        public void SetCallback(Action<short[]> callback, ObjectFactory<short[], int> bufferFactory)
        {
            this.pushCallback = callback;
            this.bufferFactory = bufferFactory;            
        }
        
        private void push(IntPtr buf, int lenBytes)
        {
            if (pushCallback != null)
            {
                var len = lenBytes / sizeof(short);
                var bufManaged = this.bufferFactory.New(len);
                Marshal.Copy(buf, bufManaged, 0, len);
                pushCallback(bufManaged);
            }
        }

        public int Channels { get { return 1; } }

        public int SamplingRate { get { return 16000; } }

        public string Error { get; private set; }

        public void Dispose()
        {
            if (handle != IntPtr.Zero)
            {
                Photon_Audio_In_Destroy(handle);
                handle = IntPtr.Zero;
            }
            // TODO: Remove this from instancePerHandle
        }
    }
}
#endif