using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the controllers
    /// </summary>
    public class GripInputCaptureSystem : ComponentSystem
    {
        private JobHandle _inputDeps;

        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (_inputDeps.IsCompleted)
            {
                GripInputCaptureJob job = new GripInputCaptureJob
                {
                    LeftGripSqueezeValue = Input.GetAxis("LeftGripSqueeze"),
                    RightGripSqueezeValue = Input.GetAxis("RightGripSqueeze"),
                    ShouldRaiseLeftEvent = new NativeArray<bool>(4, Allocator.TempJob),
                    ShouldRaiseRightEvent = new NativeArray<bool>(4, Allocator.TempJob)
                };

                _inputDeps = job.Schedule(this);
                _inputDeps.Complete();

                CheckEventToRaise(job.ShouldRaiseLeftEvent, EHand.LEFT);
                CheckEventToRaise(job.ShouldRaiseRightEvent, EHand.RIGHT);

                job.ShouldRaiseLeftEvent.Dispose();
                job.ShouldRaiseRightEvent.Dispose();
            }
        }

        private void CheckEventToRaise(NativeArray<bool> shouldRaiseEventList, EHand hand)
        {
            if (shouldRaiseEventList[0])
                new ButtonClickEvent(hand, EControllersButton.GRIP);
            else if (shouldRaiseEventList[1])
                new ButtonUnclickEvent(hand, EControllersButton.GRIP);
            else if (shouldRaiseEventList[2])
                new ButtonTouchEvent(hand, EControllersButton.GRIP);
            else if (shouldRaiseEventList[3])
                new ButtonUntouchEvent(hand, EControllersButton.GRIP);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct GripInputCaptureJob : IJobForEach<GripInputCapture, BaseInputCapture>
        {
            public float LeftGripSqueezeValue;
            public float RightGripSqueezeValue;

            // Outputs Array, way of doing it in Unity Jobs
            public NativeArray<bool> ShouldRaiseLeftEvent;
            public NativeArray<bool> ShouldRaiseRightEvent;

            public void Execute(ref GripInputCapture gripInput, ref BaseInputCapture baseInput)
            {
                // This system only works for the left controller, as the left input are given as parameters of this system
                var gripSqueezeValue = gripInput.Hand == EHand.LEFT ? LeftGripSqueezeValue : RightGripSqueezeValue;
                var output = gripInput.Hand == EHand.LEFT ? ShouldRaiseLeftEvent : ShouldRaiseRightEvent;

                // Check Click Events
                if (!baseInput.IsClicking && gripSqueezeValue > gripInput.SqueezeClickThreshold)
                {
                    baseInput.IsClicking = true;
                    baseInput.IsClicking = false;
                    output[0] = true;
                }
                else if (baseInput.IsClicking && gripSqueezeValue < gripInput.SqueezeClickThreshold)
                {
                    baseInput.IsClicking = false;
                    output[1] = true;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsTouching && gripSqueezeValue > 0.0f)
                {
                    baseInput.IsTouching = true;
                    output[2] = true;
                }
                else if (baseInput.IsTouching && gripSqueezeValue == 0.0f)
                {
                    baseInput.IsTouching = false;
                    output[3] = true;
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
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR && VRSF_Components.DeviceLoaded != EDevice.GEAR_VR && VRSF_Components.DeviceLoaded != EDevice.OCULUS_GO;
        }
        #endregion PRIVATE_METHODS
    }
}
