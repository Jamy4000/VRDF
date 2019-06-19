using Unity.Entities;
using UnityEngine;
using VRSF.Core.Events;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    /// <summary>
    /// Check if we use the button action choser script for this SDK thanks to the toggle in the Inspector.
    /// </summary>
    public class BACSDKCheckingSystem : ComponentSystem
    {
        struct Filter
        {
            public SDKChoserComponent SDKComp;
            public BACGeneralComponent BAC_General_Comp;
            public BACCalculationsComponent BAC_Calculations_Comp;
        }


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += Init;
            base.OnCreateManager();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= Init;
        }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// Check if at least one of the three toggles for the SDK to Use is set at true, and if the current loaded Device is listed in those bool
        /// </summary>
        /// <returns>true if the current loaded SDK is selected in the inspector</returns>
        private bool CheckUseSDKToggles(Filter entity)
        {
            if (!entity.SDKComp.UseVive && !entity.SDKComp.UseRift && !entity.SDKComp.UseSimulator)
            {
                Debug.LogError("<b>[VRSF] :</b> You need to chose at least one SDK to use the " + GetType().Name + " script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                entity.SDKComp.gameObject.SetActive(false);
                return false;
            }

            switch (VRSF_Components.DeviceLoaded)
            {
                case EDevice.HTC_VIVE:
                    return entity.SDKComp.UseVive;

                case EDevice.WMR:
                    return entity.SDKComp.UseWMR;

                case EDevice.OCULUS_RIFT:
                    return entity.SDKComp.UseRift;

                case EDevice.SIMULATOR:
                    return entity.SDKComp.UseSimulator;

                default:
                    return false;
            }
        }

        private void Init(OnSetupVRReady setupVRReady)
        {
            foreach (var e in GetEntities<Filter>())
            {
                // Is put in an if method as the CanBeUsed is set in other script and we don't want to set it at true (true being is default value)
                e.BAC_Calculations_Comp.CorrectSDK = CheckUseSDKToggles(e);
            }
            new SDKChoserIsSetup();
        }
        #endregion PRIVATES_METHODS
    }
}
