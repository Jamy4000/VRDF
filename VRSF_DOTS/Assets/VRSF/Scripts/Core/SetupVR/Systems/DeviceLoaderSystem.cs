using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace VRSF.Core.SetupVR
{
    public class DeviceLoaderSystem : ComponentSystem
    {
        #region JobComponentSystem_METHODS
        protected override void OnCreate()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            OnSetupVRNeedToBeReloaded.Listeners += ReloadSetupVR;
            base.OnCreate();
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            CheckEntities();
        }

        protected override void OnDestroyManager()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            OnSetupVRNeedToBeReloaded.Listeners -= ReloadSetupVR;
            base.OnDestroyManager();
        }

        protected override void OnUpdate()
        {
            if (!DeviceToLoad.IsLoaded)
                CheckEntities();
        }
        #endregion


        #region PRIVATE_METHODS
        private void CheckEntities()
        {
            Entities.ForEach((ref DeviceToLoad deviceToLoad) =>
            {
                if (string.IsNullOrEmpty(XRDevice.model))
                    LoadVRSDK(ref deviceToLoad);
            });

            this.Enabled = !DeviceToLoad.IsLoaded;
        }
        /// <summary>
        /// Load everything needed based on connected device or the one specified in the editor.
        /// </summary>
        void LoadVRSDK(ref DeviceToLoad deviceToLoad)
        {
            if (deviceToLoad.ShouldCheckConnectedDevice)
            {
                deviceToLoad.Device = HeadsetChecker.CheckDeviceConnected();
            }
            else if (deviceToLoad.Device == EDevice.NULL)
            {
                Debug.LogError("<b>[VRSF] :</b> Device to Load is null, Checking runtime device.");
                deviceToLoad.Device = HeadsetChecker.CheckDeviceConnected();
            }

            VRSF_Components.DeviceLoaded = deviceToLoad.Device;
            VRSF_Components.CameraRig.transform.name = "[VRSF] " + deviceToLoad.Device.ToString();
            DeviceToLoad.IsLoaded = true;
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
            DeviceToLoad.IsLoaded = false;
            this.Enabled = true;
        }
        #endregion
    }
}