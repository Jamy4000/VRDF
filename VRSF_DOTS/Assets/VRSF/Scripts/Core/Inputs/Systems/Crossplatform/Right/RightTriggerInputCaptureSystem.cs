using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the right controller
    /// </summary>
    public class RightTriggerInputCaptureSystem : ComponentSystem
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
                TriggerInputCaptureJob job = new TriggerInputCaptureJob
                {
                    TriggerSqueezeValue = Input.GetAxis("RightTriggerSqueeze"),
                    ShouldRaiseClickEvent = new NativeArray<bool>(1, Allocator.TempJob),
                    ShouldRaiseUnclickEvent = new NativeArray<bool>(1, Allocator.TempJob),
                    ShouldRaiseTouchEvent = new NativeArray<bool>(1, Allocator.TempJob),
                    ShouldRaiseUntouchEvent = new NativeArray<bool>(1, Allocator.TempJob),
                };

                _inputDeps = job.Schedule(this);
                _inputDeps.Complete();

                if (job.ShouldRaiseClickEvent[0])
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                else if (job.ShouldRaiseTouchEvent[0])
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                else if (job.ShouldRaiseUnclickEvent[0])
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                else if (job.ShouldRaiseUntouchEvent[0])
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);

                job.ShouldRaiseClickEvent.Dispose();
                job.ShouldRaiseTouchEvent.Dispose();
                job.ShouldRaiseUnclickEvent.Dispose();
                job.ShouldRaiseUntouchEvent.Dispose();

            }
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct TriggerInputCaptureJob : IJobForEach<TriggerInputCapture, BaseInputCapture>
        {
            public float TriggerSqueezeValue;

            // Outputs
            public NativeArray<bool> ShouldRaiseClickEvent;
            public NativeArray<bool> ShouldRaiseUnclickEvent;
            public NativeArray<bool> ShouldRaiseTouchEvent;
            public NativeArray<bool> ShouldRaiseUntouchEvent;

            public void Execute(ref TriggerInputCapture triggerInput, ref BaseInputCapture baseInput)
            {
                // This system only works for the right controller, as the right input are given as parameters of this system
                if (triggerInput.Hand == EHand.RIGHT)
                {
                    // Check Click Events
                    if (!baseInput.IsClicking && TriggerSqueezeValue > triggerInput.SqueezeClickThreshold)
                    {
                        baseInput.IsClicking = true;
                        baseInput.IsTouching= false;
                        ShouldRaiseClickEvent[0] = true;
                    }
                    else if (baseInput.IsClicking && TriggerSqueezeValue < triggerInput.SqueezeClickThreshold)
                    {
                        baseInput.IsClicking = false;
                        ShouldRaiseUnclickEvent[0] = true;
                    }
                    // Check Touch Events if user is not clicking
                    else if (!baseInput.IsTouching && TriggerSqueezeValue > 0.0f)
                    {
                        baseInput.IsTouching = true;
                        ShouldRaiseTouchEvent[0] = true;
                    }
                    else if (baseInput.IsTouching && TriggerSqueezeValue == 0.0f)
                    {
                        baseInput.IsTouching = false;
                        ShouldRaiseUntouchEvent[0] = true;
                    }
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
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
        #endregion PRIVATE_METHODS
    }
}
