using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the left controller
    /// </summary>
    public class TriggerInputCaptureSystem : ComponentSystem
    {
        private JobHandle _inputDeps;

        protected override void OnUpdate()
        {
            if (_inputDeps.IsCompleted)
            {
                TriggerInputCaptureJob job = new TriggerInputCaptureJob
                {
                    LeftTriggerSqueezeValue = Input.GetAxis("LeftTriggerSqueeze"),
                    RightTriggerSqueezeValue = Input.GetAxis("RightTriggerSqueeze"),
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
                new ButtonClickEvent(hand, EControllersButton.TRIGGER);
            else if (shouldRaiseEventList[1])
                new ButtonUnclickEvent(hand, EControllersButton.TRIGGER);
            else if (shouldRaiseEventList[2])
                new ButtonTouchEvent(hand, EControllersButton.TRIGGER);
            else if (shouldRaiseEventList[3])
                new ButtonUntouchEvent(hand, EControllersButton.TRIGGER);
        }

        [Unity.Burst.BurstCompile]
        struct TriggerInputCaptureJob : IJobForEach<TriggerInputCapture, BaseInputCapture>
        {
            public float LeftTriggerSqueezeValue;
            public float RightTriggerSqueezeValue;

            // Outputs Array, way of doing it in Unity Jobs
            public NativeArray<bool> ShouldRaiseLeftEvent;
            public NativeArray<bool> ShouldRaiseRightEvent;

            public void Execute(ref TriggerInputCapture triggerInput, ref BaseInputCapture baseInput)
            {
                if (triggerInput.Hand == EHand.LEFT)
                    CheckInputs(ref ShouldRaiseLeftEvent, baseInput, LeftTriggerSqueezeValue, triggerInput.SqueezeClickThreshold);
                else
                    CheckInputs(ref ShouldRaiseRightEvent, baseInput, RightTriggerSqueezeValue, triggerInput.SqueezeClickThreshold);
            }

            private void CheckInputs(ref NativeArray<bool> shouldRaiseEvent, BaseInputCapture baseInput, float triggerSqueezeValue, float triggerSqueezeThreshold)
            {
                // Check Click Events
                if (!baseInput.IsClicking && triggerSqueezeValue > triggerSqueezeThreshold)
                {
                    baseInput.IsClicking = true;
                    baseInput.IsTouching = false;
                    shouldRaiseEvent[0] = true;
                }
                else if (baseInput.IsClicking && triggerSqueezeValue < triggerSqueezeThreshold)
                {
                    baseInput.IsClicking = false;
                    shouldRaiseEvent[1] = true;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsTouching && triggerSqueezeValue > 0.0f)
                {
                    baseInput.IsTouching = true;
                    shouldRaiseEvent[2] = true;
                }
                else if (baseInput.IsTouching && triggerSqueezeValue == 0.0f)
                {
                    baseInput.IsTouching = false;
                    shouldRaiseEvent[3] = true;
                }
            }
        }
    }
}
