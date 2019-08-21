using UnityEngine;
using System.Collections.Generic;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Raycast
{
    public static class ControllersRaycastOffset
    {
        public static Dictionary<EDevice, Vector3> RaycastPositionOffset = new Dictionary<EDevice, Vector3>
        {
            { EDevice.GEAR_VR, Vector3.zero },
            { EDevice.HTC_FOCUS, Vector3.zero },
            { EDevice.HTC_VIVE, Vector3.zero },
            { EDevice.OCULUS_GO, Vector3.zero },
            { EDevice.OCULUS_QUEST, Vector3.zero },
            { EDevice.OCULUS_RIFT, Vector3.zero },
            { EDevice.OCULUS_RIFT_S, Vector3.zero },
            { EDevice.SIMULATOR, Vector3.zero },
            { EDevice.WMR, new Vector3(-0.01f, -0.02f, -0.01f)}
        };

        public static Dictionary<EDevice, Vector3> RaycastDirectionOffset = new Dictionary<EDevice, Vector3>
        {
            { EDevice.GEAR_VR, Vector3.zero },
            { EDevice.HTC_FOCUS, Vector3.zero },
            { EDevice.HTC_VIVE, Vector3.zero },
            { EDevice.OCULUS_GO, Vector3.zero },
            { EDevice.OCULUS_QUEST, Vector3.zero },
            { EDevice.OCULUS_RIFT, Vector3.zero },
            { EDevice.OCULUS_RIFT_S, Vector3.zero },
            { EDevice.SIMULATOR, Vector3.zero },
            { EDevice.WMR, new Vector3(-0.015f, -0.5f, 0.0f)}
        };
    }
}