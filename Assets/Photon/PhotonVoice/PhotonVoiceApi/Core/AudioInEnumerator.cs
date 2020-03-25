﻿#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
#if (UNITY_IOS && !UNITY_EDITOR) || __IOS__
#define DLL_IMPORT_INTERNAL
#endif
using System;
using System.Runtime.InteropServices;
namespace Photon.Voice
{
    /// <summary>Enumerates microphones available on device.
    /// </summary>
    public class AudioInEnumerator : IDisposable
    {
#if DLL_IMPORT_INTERNAL
	const string lib_name = "__Internal";
#else
        const string lib_name = "AudioIn";
#endif
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_CreateMicEnumerator();
        [DllImport(lib_name)]
        private static extern void Photon_Audio_In_DestroyMicEnumerator(IntPtr handle);
        [DllImport(lib_name)]
        private static extern int Photon_Audio_In_MicEnumerator_Count(IntPtr handle);
        [DllImport(lib_name)]
        private static extern IntPtr Photon_Audio_In_MicEnumerator_NameAtIndex(IntPtr handle, int idx);
        [DllImport(lib_name)]
        private static extern int Photon_Audio_In_MicEnumerator_IDAtIndex(IntPtr handle, int idx);
        IntPtr handle;
        public AudioInEnumerator(ILogger logger)
        {            
            Refresh();
            if (Error != null)
            {
                logger.LogError("[PV] AudioInEnumerator: " + Error);
            }
        }
        /// <summary>Refreshes the microphones list.
        /// </summary>
        public void Refresh()
        {
            Dispose();
            try
            {
                handle = Photon_Audio_In_CreateMicEnumerator();
                Error = null;
            }
            catch(Exception e)
            {
                Error = e.ToString();
                if (Error == null) // should never happen but since Error used as validity flag, make sure that it's not null
                {
                    Error = "Exception in AudioInEnumerator.Refresh()";
                }
            }
        }
        /// <summary>True if enumeration supported for the current platform.</summary>
        public readonly bool IsSupported = true;
        /// <summary>If not null, the enumerator is in invalid state.</summary>
        public string Error { get; private set; }
        /// <summary>Returns the count of microphones available on the device.
        /// </summary>
        /// <returns>Microphones count.</returns>
        public int Count { get { return Error == null ? Photon_Audio_In_MicEnumerator_Count(handle) : 0; } }
        /// <summary>Returns the microphone name by index in the microphones list.
        /// </summary>
        /// <param name="idx">Position in the list.</param>
        /// <returns>Microphone ID.</returns>
        public string NameAtIndex(int idx)
        {
            return Error == null ? Marshal.PtrToStringAuto(Photon_Audio_In_MicEnumerator_NameAtIndex(handle, idx)) : "";
        }
        /// <summary>Returns the microphone ID by index in the microphones list.
        /// </summary>
        /// <param name="idx">Position in the list.</param>
        /// <returns>Microphone name.</returns>
        public int IDAtIndex(int idx)
        {
            return Error == null ? Photon_Audio_In_MicEnumerator_IDAtIndex(handle, idx) : -2;
        }
        /// <summary>Checks if microphone with given ID exists.
        /// </summary>
        /// <param name="id">Microphone ID to check.</param>
        /// <returns>True if ID is valid.</returns>
        public bool IDIsValid(int id)
        {
            return id >= -1;// TODO: cache devices IDs and check the value against the cache
        }
        /// <summary>Disposes enumerator.
        /// Call it to free native resources.
        /// </summary>
        public void Dispose()
        {
            if (handle != IntPtr.Zero && Error == null)
            {
                Photon_Audio_In_DestroyMicEnumerator(handle);
                handle = IntPtr.Zero;
            }
        }
    }
}
#else
using System;
namespace Photon.Voice
{
    // Stub for platform not supporting mic enumeration
    // 
    public class AudioInEnumerator : IDisposable
    {
        public readonly bool IsSupported = false;
        public AudioInEnumerator(ILogger logger)
        {
        }
        public void Refresh()
        {
        }
        public string Error { get { return "Current platform is not supported by AudioInEnumerator."; } }
        public int Count { get { return 0; } }
        public string NameAtIndex(int i)
        {
            return null;
        }
        public int IDAtIndex(int i)
        {
            return -1;
        }
        public bool IDIsValid(int id)
        {
            return id >= -1;
        }
        public void Dispose()
        {
        }
    }
}
#endif
