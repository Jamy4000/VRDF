using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the left controllers
    /// </summary>
    public class CrossplatformLeftGripInputCaptureSystem : JobComponentSystem
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
                GripSqueezeValue = Input.GetAxis("LeftGripSqueeze")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct GripInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public float GripSqueezeValue;

            public void Execute(ref CrossplatformInputCapture crossplatformInput)
            {
                // Check Click Events
                if (!LeftInputsParameters.GripClick && GripSqueezeValue > crossplatformInput.SqueezeClickThreshold)
                {
                    LeftInputsParameters.GripClick = true;
                    LeftInputsParameters.GripTouch = false;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
                }
                else if (LeftInputsParameters.GripClick && GripSqueezeValue < crossplatformInput.SqueezeClickThreshold)
                {
                    LeftInputsParameters.GripClick = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
                }
                // Check Touch Events if user is not clicking
                else if (!LeftInputsParameters.GripTouch && GripSqueezeValue > 0.0f)
                {
                    LeftInputsParameters.GripTouch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.GRIP);
                }
                else if (LeftInputsParameters.GripTouch && GripSqueezeValue == 0.0f)
                {
                    LeftInputsParameters.GripTouch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.GRIP);
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
