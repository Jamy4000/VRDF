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
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new ThumbrestButtonInputCaptureJob()
            {
                ThumbrestTouchButtonDown = Input.GetButtonDown("OculusLeftThumbrestTouch"),
                ThumbrestTouchButtonUp = Input.GetButtonUp("OculusLeftThumbrestTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct ThumbrestButtonInputCaptureJob : IJobForEach<CrossplatformInputCapture>
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