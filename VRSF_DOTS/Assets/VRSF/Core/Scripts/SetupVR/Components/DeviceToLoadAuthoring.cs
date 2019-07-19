using System;
using UnityEngine;
using UnityEngine.SpatialTracking;
using VRSF.Core.FadingEffect;

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

            if (Device == EDevice.SIMULATOR)
                RemoveVRStuffs();

            VRSF_Components.SetupVRIsReady = true;
            new OnSetupVRReady();
            new StartFadingInEvent(0.5f);
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