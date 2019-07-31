using UnityEngine;
using UnityEngine.SpatialTracking;

namespace VRSF.Core.SetupVR
{ 
    public class DeviceToLoadAuthoring : MonoBehaviour
    {
        [Header("VR Device Parameters.")]
        [Tooltip("The Device you want to load.")]
        [SerializeField]
        public EDevice Device = EDevice.SIMULATOR;

        [Tooltip("If false, the device to load will be set with your Editor choice or with a potential starting screen choice.")]
        [SerializeField]
        public bool CheckDeviceAtRuntime = true;

        public void Start()
        {
            if (CheckDeviceAtRuntime)
            {
                Device = HeadsetChecker.CheckDeviceConnected();
            }
            else if (Device == EDevice.NONE || Device == EDevice.ALL)
            {
                Debug.LogError("<b>[VRSF] :</b> Device to Load is null, Checking runtime device.");
                Device = HeadsetChecker.CheckDeviceConnected();
            }

            VRSF_Components.DeviceLoaded = Device;
            VRSF_Components.CameraRig.transform.name = "[VRSF] " + Device.ToString();

            // SteamVR Do not use any floor offset, so we reduce it of floorOffsetY meter * scale if this is the loaded SDK
            VRSF_Components.FloorOffset.transform.localPosition = UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR" ? CalculateNewFloorOffset() : VRSF_Components.FloorOffset.transform.localPosition;

            if (Device == EDevice.SIMULATOR)
                RemoveVRStuffs();

            VRSF_Components.SetupVRIsReady = true;
            new OnSetupVRReady();
        }

        private Vector3 CalculateNewFloorOffset()
        {
            Debug.Log("CalculateNewFloorOffset");
            var localPos = VRSF_Components.FloorOffset.transform.localPosition;
            return new Vector3(localPos.x, localPos.y - (localPos.y * VRSF_Components.FloorOffset.transform.lossyScale.y), localPos.z);
        }

        private void RemoveVRStuffs()
        {
            Destroy(VRSF_Components.LeftController.GetComponent<TrackedPoseDriver>());
            Destroy(VRSF_Components.RightController.GetComponent<TrackedPoseDriver>());
            Destroy(VRSF_Components.VRCamera.GetComponent<TrackedPoseDriver>());
            UnityEngine.XR.XRSettings.enabled = false;
        }
    }
}