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
    public class OculusLeftThumbrestInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var touchpadJob = new ThumbrestButtonInputCapture()
            {
                ThumbrestTouchButtonDown = Input.GetButtonDown("OculusLeftThumbrestTouch"),
                ThumbrestTouchButtonUp = Input.GetButtonUp("OculusLeftThumbrestTouch")
            };

            return touchpadJob.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct ThumbrestButtonInputCapture : IJobForEach<CrossplatformInputCapture>
        {
            public bool ThumbrestTouchButtonDown;
            public bool ThumbrestTouchButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                if (ThumbrestTouchButtonDown)
                {
                    LeftInputsParameters.ThumbrestTouch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
                }
                else if (ThumbrestTouchButtonUp)
                {
                    LeftInputsParameters.ThumbrestTouch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
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