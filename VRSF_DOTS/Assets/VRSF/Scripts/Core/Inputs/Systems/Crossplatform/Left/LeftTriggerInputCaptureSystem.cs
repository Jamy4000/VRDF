using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the left controller
    /// </summary>
    public class LeftTriggerInputCaptureSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TriggerInputCaptureJob
            {
                TriggerSqueezeValue = Input.GetAxis("LeftTriggerSqueeze")
            }.Schedule(this, inputDeps);
        }

        struct TriggerInputCaptureJob : IJobForEach<TriggerInputCapture, BaseInputCapture>
        {
            public float TriggerSqueezeValue;

            public void Execute(ref TriggerInputCapture triggerInput, ref BaseInputCapture baseInput)
            {
                // This system only works for the left controller, as the left input are given as parameters of this system
                if (triggerInput.Hand == EHand.LEFT)
                {
                    // Check Click Events
                    if (!baseInput.IsClicking && TriggerSqueezeValue > triggerInput.SqueezeClickThreshold)
                    {
                        baseInput.IsClicking = true;
                        baseInput.IsTouching = false;
                        new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                    else if (baseInput.IsClicking && TriggerSqueezeValue < triggerInput.SqueezeClickThreshold)
                    {
                        baseInput.IsClicking = false;
                        new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!baseInput.IsTouching && TriggerSqueezeValue > 0.0f)
                    {
                        baseInput.IsTouching = true;
                        new ButtonTouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                    else if (baseInput.IsTouching && TriggerSqueezeValue == 0.0f)
                    {
                        baseInput.IsTouching = false;
                        new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                }
            }
        }
    }
}
