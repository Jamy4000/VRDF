using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;

namespace VRDF.Core.SetupVR
{ 
    public class DeviceToLoadAuthoring : MonoBehaviour
    {
        [Header("VR Device Parameters.")]
        [Tooltip("The Device you want to load.")]
        [SerializeField]
        private EDevice _device = EDevice.SIMULATOR;

        [Tooltip("If false, the device to load will be set with your Editor choice or with a potential starting screen choice.")]
        [SerializeField]
        private bool _checkDeviceAtRuntime = true;

        [Header("SDK Forcing (WARNING : For Experienced Users Only)")]
        [Tooltip("If you want to force one specific sdk (OpenVR, Oculus or None).")]
        public bool ForceSDKLoad;

        [HideInInspector] public EVR_SDK ForcedSDK = EVR_SDK.OPEN_VR;

        private void Awake()
        {
            VRDF_Components.SetupVRIsReady = false;
        }

        public void Start()
        {
            if (_checkDeviceAtRuntime)
            {
                _device = HeadsetChecker.CheckDeviceConnected();
            }
            else if (_device == EDevice.NONE || _device == EDevice.ALL)
            {
                Debug.LogError("<b>[VRDF] :</b> Device to Load is not valid, Checking runtime device.");
                _device = HeadsetChecker.CheckDeviceConnected();
            }

            VRDF_Components.DeviceLoaded = _device;
            VRDF_Components.CameraRig.transform.name = "[VRDF] " + _device.ToString();

            // SteamVR Do not use any floor offset, so we reduce it of floorOffsetY meter * scale if this is the loaded SDK
            if (XRSettings.loadedDeviceName == "OpenVR" && _device != EDevice.SIMULATOR) 
                VRDF_Components.FloorOffset.transform.localPosition = CalculateNewFloorOffset();

            if (ForceSDKLoad)
                TrySDKForcing();

            if (_device == EDevice.SIMULATOR)
                RemoveVRStuffs();
            else
                XRSettings.enabled = true;

            VRDF_Components.SetupVRIsReady = true;
            new OnSetupVRReady();
        }

        private void TrySDKForcing()
        {
            switch (ForcedSDK)
            {
                case EVR_SDK.OCULUS:
                    if (_device == EDevice.HTC_FOCUS || _device == EDevice.HTC_VIVE || _device == EDevice.SIMULATOR || _device == EDevice.WMR)
                        Debug.LogErrorFormat("<b>[VRDF] :</b> Trying to force Oculus SDK, but {0} is the loaded Device. Please check your settings on SetupVR.", _device.ToString());

                    XRSettings.LoadDeviceByName("Oculus");
                    break;
                case EVR_SDK.OPEN_VR:
                    if (_device == EDevice.SIMULATOR)
                        Debug.LogError("<b>[VRDF] :</b> Trying to force OpenVR SDK, but Simulator is the loaded Device. Please check your settings on SetupVR.");

                    XRSettings.LoadDeviceByName("OpenVR");
                    break;
                default:
                    XRSettings.LoadDeviceByName("None");
                    break;
            }
        }

        private Vector3 CalculateNewFloorOffset()
        {
            var localPos = VRDF_Components.FloorOffset.transform.localPosition;
            return new Vector3(localPos.x, localPos.y - (localPos.y * VRDF_Components.FloorOffset.transform.lossyScale.y), localPos.z);
        }

        private void RemoveVRStuffs()
        {
            Destroy(VRDF_Components.LeftController.GetComponent<TrackedPoseDriver>());
            Destroy(VRDF_Components.RightController.GetComponent<TrackedPoseDriver>());
            Destroy(VRDF_Components.VRCamera.GetComponent<TrackedPoseDriver>());
        }
    }
}