using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the right controllers
    /// </summary>
    public class CrossplatformRightGripInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new GripInputCaptureJob()
            {
                GripSqueezeValue = Input.GetAxis("RightGripSqueeze")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct GripInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public float GripSqueezeValue;

            public void Execute(ref CrossplatformInputCapture crossplatformInput)
            {
                // Check Click Events
                if (!RightInputsParameters.GripClick && GripSqueezeValue > crossplatformInput.SqueezeClickThreshold)
                {
                    RightInputsParameters.GripClick = true;
                    RightInputsParameters.GripTouch = false;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
                }
                else if (RightInputsParameters.GripClick && GripSqueezeValue < crossplatformInput.SqueezeClickThreshold)
                {
                    RightInputsParameters.GripClick = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
                }
                // Check Touch Events if user is not clicking
                else if (!RightInputsParameters.GripTouch && GripSqueezeValue > 0.0f)
                {
                    RightInputsParameters.GripTouch = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.GRIP);
                }
                else if (RightInputsParameters.GripTouch && GripSqueezeValue == 0.0f)
                {
                    RightInputsParameters.GripTouch = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.GRIP);
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Do not activate this system for the Simulator Go and GearVR
        /// </summary>
        /// <param name="info"></param>
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR && VRSF_Components.DeviceLoaded != EDevice.GEAR_VR && VRSF_Components.DeviceLoaded != EDevice.OCULUS_GO;
        }
        #endregion PRIVATE_METHODS
    }
}
