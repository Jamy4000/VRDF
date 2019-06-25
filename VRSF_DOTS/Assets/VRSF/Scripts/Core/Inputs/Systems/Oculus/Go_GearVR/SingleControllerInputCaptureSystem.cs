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
            OnSetupVRReady.Listeners += CheckForComponents;
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
            OnSetupVRReady.Listeners -= CheckForComponents;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct BackButtonInputCaptureJob : IJobForEach<GoAndGearVRInputCapture>
        {
            public bool MenuButtonDown;
            public bool MenuButtonUp;

            public void Execute(ref GoAndGearVRInputCapture goAndGearInput)
            {
                // Check Click Events
                if (MenuButtonDown)
                {
                    goAndGearInput.BackButtonClick = true;
                    new ButtonClickEvent(goAndGearInput.IsUserRightHanded ? EHand.RIGHT : EHand.LEFT, EControllersButton.BACK_BUTTON);
                }
                else if (MenuButtonUp)
                {
                    goAndGearInput.BackButtonClick = false;
                    new ButtonUnclickEvent(goAndGearInput.IsUserRightHanded ? EHand.RIGHT : EHand.LEFT, EControllersButton.BACK_BUTTON);
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's at least one AButtonInputCapture component and that it has the RIGHT as Hand
        /// </summary>
        /// <param name="info"></param>
        private void CheckForComponents(OnSetupVRReady info)
        {
            this.Enabled = IsSingleControllerHeadset() && GetEntityQuery(typeof(GoAndGearVRInputCapture)).CalculateLength() > 0;

            bool IsSingleControllerHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.GEAR_VR || VRSF_Components.DeviceLoaded == EDevice.OCULUS_GO;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
