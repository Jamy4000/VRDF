using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusMenuButtonInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new MenuButtonInputCaptureJob()
            {
                MenuClickButtonDown = Input.GetButtonDown("OculusMenuButtonClick"),
                MenuClickButtonUp = Input.GetButtonUp("OculusMenuButtonClick"),
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct MenuButtonInputCaptureJob : IJobForEach<MenuInputCapture, BaseInputCapture>
        {
            public bool MenuClickButtonDown;
            public bool MenuClickButtonUp;

            public void Execute(ref MenuInputCapture menuInput, ref BaseInputCapture baseInput)
            {
                if (menuInput.Hand == EHand.LEFT)
                {
                    // Check Click Events
                    if (MenuClickButtonDown)
                    {
                        baseInput.IsClicking = true;
                        new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
                    }
                    else if (MenuClickButtonUp)
                    {
                        baseInput.IsClicking = false;
                        new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if we use the good device
        /// </summary>
        /// <param name="info"></param>
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = IsOculusHeadset();

            bool IsOculusHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
