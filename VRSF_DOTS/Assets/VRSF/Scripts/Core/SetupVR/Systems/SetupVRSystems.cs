using System;
using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.SetupVR
{
    public class SetupVRSystems : ComponentSystem
    {
        /// <summary>
        /// The filter to find SetupVR entity
        /// </summary>
        struct Filter
        {
            public SetupVRComponents SetupVR;
        }

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSetupVRNeedToBeReloaded.Listeners += ReloadSetupVR;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            foreach (var e in GetEntities<Filter>())
            {
                LoadCorrespondingSDK(e.SetupVR);
                SetupVRInScene(e.SetupVR);
            }
            
            this.Enabled = !VRSF_Components.SetupVRIsReady;
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (!e.SetupVR.IsLoaded && XRDevice.model != String.Empty)
                    LoadCorrespondingSDK(e.SetupVR);
                else if (e.SetupVR.IsLoaded && !VRSF_Components.SetupVRIsReady)
                    SetupVRInScene(e.SetupVR);

                this.Enabled = !VRSF_Components.SetupVRIsReady;
            }
        }
        
        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnSetupVRNeedToBeReloaded.Listeners -= ReloadSetupVR;
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Will Instantiate and reference the SDK prefab to load thanks to the string field.
        /// </summary>
        void LoadCorrespondingSDK(SetupVRComponents setupVR)
        {
            if (setupVR.CheckDeviceAtRuntime)
            {
                setupVR.DeviceToLoad = HeadsetChecker.CheckDeviceConnected();
            }
            else if (setupVR.DeviceToLoad == EDevice.NULL)
            {
                Debug.LogError("<b>[VRSF] :</b> Device to Load is null, Checking runtime device.");
                setupVR.DeviceToLoad = HeadsetChecker.CheckDeviceConnected();
            }

            VRSF_Components.DeviceLoaded = setupVR.DeviceToLoad;

            // Setup the Input scripts, Controllers meshes and start position based on the SDK loaded.
            SpecificHeadsetComponents.Setup(setupVR);

            setupVR.CameraRig.transform.name = "[VRSF] " + setupVR.DeviceToLoad.ToString();
            setupVR.CameraRig.transform.SetParent(null);
            setupVR.IsLoaded = true;
        }


        /// <summary>
        /// Method called on Awake and in Update, if the setup is not finished, 
        /// to load the VR SDK Prefab and set its parameters.
        /// </summary>
        private void SetupVRInScene(SetupVRComponents setupVR)
        {
            if (setupVR.FloorOffset == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No Floor Offset was references in SetupVR. Trying to fetch it using the name Floor_Offset.");
                setupVR.FloorOffset = GameObject.Find("Floor_Offset").transform;
                return;
            }

            VRSF_Components.FloorOffset = setupVR.FloorOffset;

            if (setupVR.CameraRig == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No CameraRig was references in SetupVR. Trying to fetch it using tag RESERVED_CameraRig.");
                setupVR.CameraRig = GameObject.FindGameObjectWithTag("RESERVED_CameraRig");
                return;
            }

            VRSF_Components.CameraRig = setupVR.CameraRig;

            // We set the references to the VRCamera
            if (setupVR.VRCamera == null)
            {
                Debug.LogError("<b>[VRSF] :</b> No VRCamera was references in SetupVR. Trying to fetch it using tag MainCamera.");
                setupVR.VRCamera = GameObject.FindGameObjectWithTag("MainCamera");
                return;
            }

            VRSF_Components.VRCamera = setupVR.VRCamera;

            VRSF_Components.LeftController = setupVR.LeftController;
            VRSF_Components.RightController = setupVR.RightController;

            VRSF_Components.SetupVRIsReady = true;
            new OnSetupVRReady();
        }


        /// <summary>
        /// Disable the two controllers if we don't use them
        /// </summary>
        /// <returns>true if the controllers were disabled correctly</returns>
        bool DisableControllers(SetupVRComponents setupVR)
        {
            try
            {
                switch (VRSF_Components.DeviceLoaded)
                {
                    case EDevice.OCULUS_RIFT:
                    case EDevice.OCULUS_QUEST:
                    case EDevice.OCULUS_RIFT_S:
                        setupVR.CameraRig.GetComponent<OculusControllersInputCaptureComponent>().enabled = false;
                        break;
                    case EDevice.OCULUS_GO:
                    case EDevice.GEAR_VR:
                        setupVR.CameraRig.GetComponent<GoAndGearVRControllersInputCaptureComponent>().enabled = false;
                        break;
                    case EDevice.HTC_VIVE:
                    case EDevice.HTC_FOCUS:
                        setupVR.CameraRig.GetComponent<HtcControllersInputCaptureComponent>().enabled = false;
                        break;
                    case EDevice.WMR:
                        setupVR.CameraRig.GetComponent<WMRControllersInputCaptureComponent>().enabled = false;
                        break;
                    case EDevice.SIMULATOR:
                        setupVR.CameraRig.GetComponent<SimulatorInputCaptureComponent>().enabled = false;
                        break;
                    default:
                        Debug.LogErrorFormat("<b>[VRSF] :</b> Device Loaded is not set to a valid value : {0}", VRSF_Components.DeviceLoaded);
                        return false;
                }

                return true;
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("<b>[VRSF] :</b> Can't disable Left and Right Controllers.\n{0}", e.ToString());
                return false;
            }
        }


        /// <summary>
        /// Reactivate the System when switching to another Scene.
        /// </summary>
        /// <param name="oldScene">The previous scene before switching</param>
        private void OnSceneLoaded(Scene oldScene, LoadSceneMode loadMode)
        {
            if (loadMode == LoadSceneMode.Single)
                new OnSetupVRNeedToBeReloaded();
        }


        private void ReloadSetupVR(OnSetupVRNeedToBeReloaded info)
        {
            VRSF_Components.SetupVRIsReady = false;
            this.Enabled = true;
        }
#endregion
    }
}