using UnityEngine;
using UnityEngine.XR;

namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Helpers class to check which HMD is connected at runtime
    /// </summary>
    public static class HeadsetChecker
    {
        /// <summary>
        /// Check which device is connected, and set the DeviceToLoad to the right name.
        /// </summary>
        public static EDevice CheckDeviceConnected()
        {
            if (XRDevice.isPresent)
            {
                string detectedHmd = XRDevice.model;

                Debug.LogFormat("<b>[VRSF] :</b> {0} is connected.", detectedHmd);

                if (detectedHmd.ToLower().Contains("htc"))
                    return CheckHtcHeadset(detectedHmd);
                else if (detectedHmd.ToLower().Contains("gear"))
                    return EDevice.GEAR_VR;
                else if (detectedHmd.ToLower().Contains("oculus"))
                    return CheckOculusHeadset(detectedHmd);
                else if (detectedHmd.ToLower().Contains("windows"))
                    return EDevice.WMR;
                else
                    return LoadSimulatorWithDebugText("<b>[VRSF] :</b> " + detectedHmd + " is not supported yet, loading Simulator Support.", true);
            }
            else
            {
                return LoadSimulatorWithDebugText("<b>[VRSF] :</b> No XRDevice present, loading Simulator");
            }
        }
        
        private static EDevice CheckHtcHeadset(string detectedHmd)
        {
            if (detectedHmd.ToLower().Contains("vive"))
                return EDevice.HTC_VIVE;
            else if (detectedHmd.ToLower().Contains("focus"))
                return EDevice.HTC_FOCUS;
            else
                Debug.LogErrorFormat("<b>[VRSF] :</b> {0} isn't supported for now. Loading HTC Vive Support.", detectedHmd);

            return EDevice.HTC_VIVE;
        }

        private static EDevice CheckOculusHeadset(string detectedHmd)
        {
#if UNITY_ANDROID
            if (detectedHmd.ToLower().Contains("quest"))
                return EDevice.OCULUS_QUEST;
            else if (detectedHmd.ToLower().Contains("go"))
                return EDevice.OCULUS_GO;
#else
            if (detectedHmd.ToLower().Contains("rift s"))
                return EDevice.OCULUS_RIFT_S;
            else if (detectedHmd.ToLower().Contains("rift"))
                return EDevice.OCULUS_RIFT;
#endif
            else
                Debug.LogErrorFormat("<b>[VRSF] :</b> {0} isn't supported for now. Loading Oculus Rift Support.", detectedHmd);

            return EDevice.OCULUS_RIFT;
        }


        private static EDevice LoadSimulatorWithDebugText(string debugText, bool isError = false)
        {
            if (isError)
                Debug.LogError(debugText);
            else
                Debug.Log(debugText);

            return EDevice.SIMULATOR;
        }
    }
}