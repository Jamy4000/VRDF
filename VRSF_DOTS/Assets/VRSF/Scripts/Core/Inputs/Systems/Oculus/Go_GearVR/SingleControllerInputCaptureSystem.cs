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
            OnSetupVRReady.Listeners += CheckDevice;
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
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct BackButtonInputCaptureJob : IJobForEach<GoAndGearVRControllersInputCaptureComponent>
        {
            public bool MenuButtonDown;
            public bool MenuButtonUp;

            public void Execute(ref GoAndGearVRControllersInputCaptureComponent c0)
            {
                // Check Click Events
                if (MenuButtonDown)
                {
                    if (c0.IsUserRightHanded)
                    {
                        RightInputsParameters.BackButtonClick = true;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.BACK_BUTTON);
                    }
                    else
                    {
                        LeftInputsParameters.BackButtonClick = true;
                        new ButtonClickEvent(EHand.LEFT, EControllersButton.BACK_BUTTON);
                    }
                }
                else if (MenuButtonUp)
                {
                    if (c0.IsUserRightHanded)
                    {
                        RightInputsParameters.BackButtonClick = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.BACK_BUTTON);
                    }
                    else
                    {
                        LeftInputsParameters.BackButtonClick = false;
                        new ButtonUnclickEvent(EHand.LEFT, EControllersButton.BACK_BUTTON);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        private void CheckDevice(OnSetupVRReady info)
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
