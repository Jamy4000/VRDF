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
    public class RightTriggerInputCaptureSystem : JobComponentSystem
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
                TriggerSqueezeValue = Input.GetAxis("RightTriggerSqueeze")
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
                // This system only works for the right controller, as the right input are given as parameters of this system
                if (triggerInput.Hand == EHand.RIGHT)
                {
                    // Check Click Events
                    if (!triggerInput.TriggerClick && TriggerSqueezeValue > triggerInput.SqueezeClickThreshold)
                    {
                        triggerInput.TriggerClick = true;
                        triggerInput.TriggerTouch = false;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                    }
                    else if (triggerInput.TriggerClick && TriggerSqueezeValue < triggerInput.SqueezeClickThreshold)
                    {
                        triggerInput.TriggerClick = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!triggerInput.TriggerTouch && TriggerSqueezeValue > 0.0f)
                    {
                        triggerInput.TriggerTouch = true;
                        new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                    }
                    else if (triggerInput.TriggerTouch && TriggerSqueezeValue == 0.0f)
                    {
                        triggerInput.TriggerTouch = false;
                        new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's at least one TriggerInputCapture component and that it has the RIGHT as Hand
        /// </summary>
        /// <param name="info"></param>
        private void CheckForComponents(OnSetupVRReady info)
        {
            var entityQuery = GetEntityQuery(typeof(TriggerInputCapture)).ToComponentDataArray<TriggerInputCapture>(Unity.Collections.Allocator.TempJob, out JobHandle jobHandle);
            if (entityQuery.Length > 0)
            {
                foreach (var tic in entityQuery)
                {
                    if (tic.Hand == EHand.RIGHT)
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
