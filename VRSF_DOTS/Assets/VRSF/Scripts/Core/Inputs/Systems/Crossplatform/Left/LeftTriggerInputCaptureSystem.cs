using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the left controller
    /// </summary>
    public class LeftTriggerInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckForComponents;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TriggerInputCaptureJob
            {
                TriggerSqueezeValue = Input.GetAxis("LeftTriggerSqueeze")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForComponents;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct TriggerInputCaptureJob : IJobForEach<TriggerInputCapture>
        {
            public float TriggerSqueezeValue;

            public void Execute(ref TriggerInputCapture triggerInput)
            {
                // This system only works for the left controller, as the left input are given as parameters of this system
                if (triggerInput.Hand == EHand.LEFT)
                {
                    // Check Click Events
                    if (!triggerInput.TriggerClick && TriggerSqueezeValue > triggerInput.SqueezeClickThreshold)
                    {
                        triggerInput.TriggerClick = true;
                        triggerInput.TriggerTouch = false;
                        new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                    else if (triggerInput.TriggerClick && TriggerSqueezeValue < triggerInput.SqueezeClickThreshold)
                    {
                        triggerInput.TriggerClick = false;
                        new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!triggerInput.TriggerTouch && TriggerSqueezeValue > 0.0f)
                    {
                        triggerInput.TriggerTouch = true;
                        new ButtonTouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                    else if (triggerInput.TriggerTouch && TriggerSqueezeValue == 0.0f)
                    {
                        triggerInput.TriggerTouch = false;
                        new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's at least one TriggerInputCapture component and that it has the LEFT as Hand
        /// </summary>
        /// <param name="info"></param>
        private void CheckForComponents(OnSetupVRReady info)
        {
            var entityQuery = GetEntityQuery(typeof(TriggerInputCapture)).ToComponentDataArray<TriggerInputCapture>(Unity.Collections.Allocator.TempJob, out JobHandle jobHandle);
            if (entityQuery.Length > 0)
            {
                foreach (var tic in entityQuery)
                {
                    if (tic.Hand == EHand.LEFT)
                    {
                        this.Enabled = true;
                        jobHandle.Complete();
                        entityQuery.Dispose();
                        return;
                    }
                }
            }
            jobHandle.Complete();
            entityQuery.Dispose();
            this.Enabled = false;
        }
        #endregion PRIVATE_METHODS
    }
}
