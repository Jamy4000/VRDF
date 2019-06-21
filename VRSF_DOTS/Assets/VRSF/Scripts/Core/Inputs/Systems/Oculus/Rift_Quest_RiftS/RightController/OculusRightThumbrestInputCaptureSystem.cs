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
    public class OculusRightThumbrestInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new ThumbrestButtonInputCaptureJob()
            {
                ThumbrestTouchButtonDown = Input.GetButtonDown("OculusRightThumbrestTouch"),
                ThumbrestTouchButtonUp = Input.GetButtonUp("OculusRightThumbrestTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct ThumbrestButtonInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public bool ThumbrestTouchButtonDown;
            public bool ThumbrestTouchButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                if (ThumbrestTouchButtonDown)
                {
                    RightInputsParameters.ThumbrestTouch = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
                }
                else if (ThumbrestTouchButtonUp)
                {
                    RightInputsParameters.ThumbrestTouch = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
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