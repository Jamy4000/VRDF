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

        [Unity.Burst.BurstCompile]
        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct MenuButtonInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public bool MenuClickButtonDown;
            public bool MenuClickButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (MenuClickButtonDown)
                {
                    LeftInputsParameters.MenuClick = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
                }
                else if (MenuClickButtonUp)
                {
                    LeftInputsParameters.MenuClick = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
                }
            }
        }

        #region PRIVATE_METHODS
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
