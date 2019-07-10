using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Only capturing the Back Button on the GearVR and the Oculus Go controller
    /// </summary>
    public class SignleControllerInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckForDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new BackButtonInputCaptureJob()
            {
                MenuButtonDown = Input.GetButtonDown("BackButtonClick"),
                MenuButtonUp = Input.GetButtonUp("BackButtonClick")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForDevice;
            base.OnDestroy();
        }

        struct BackButtonInputCaptureJob : IJobForEach<GoAndGearVRInputCapture, BaseInputCapture>
        {
            public bool MenuButtonDown;
            public bool MenuButtonUp;

            public void Execute(ref GoAndGearVRInputCapture goAndGearInput, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (MenuButtonDown)
                {
                    baseInput.IsClicking = true;
                    new ButtonClickEvent(goAndGearInput.IsUserRightHanded ? EHand.RIGHT : EHand.LEFT, EControllersButton.BACK_BUTTON);
                }
                else if (MenuButtonUp)
                {
                    baseInput.IsClicking = false;
                    new ButtonUnclickEvent(goAndGearInput.IsUserRightHanded ? EHand.RIGHT : EHand.LEFT, EControllersButton.BACK_BUTTON);
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if we use the good device
        /// </summary>
        /// <param name="info"></param>
        private void CheckForDevice(OnSetupVRReady info)
        {
            this.Enabled = IsSingleControllerHeadset();

            bool IsSingleControllerHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.GEAR_VR || VRSF_Components.DeviceLoaded == EDevice.OCULUS_GO;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
