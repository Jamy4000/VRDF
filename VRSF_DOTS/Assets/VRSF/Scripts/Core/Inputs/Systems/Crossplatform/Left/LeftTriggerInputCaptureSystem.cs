using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the left controller
    /// </summary>
    public class LeftTriggerInputCaptureSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

        protected override void OnCreate()
        {
            // Cache the EndSimulationEntityCommandBufferSystem in a field, so we don't have to get it every frame
            _endSimEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TriggerInputCaptureJob
            {
                LeftTriggerSqueezeValue = Input.GetAxis("LeftTriggerSqueeze"),
                Commands = _endSimEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this);
        }

        [RequireComponentTag(typeof(LeftHand))]
        struct TriggerInputCaptureJob : IJobForEachWithEntity<TriggerInputCapture, BaseInputCapture>
        {
            public float LeftTriggerSqueezeValue;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref TriggerInputCapture triggerInput, ref BaseInputCapture baseClickInput)
            {
                if (!baseClickInput.IsClicking && LeftTriggerSqueezeValue > triggerInput.SqueezeClickThreshold)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.TRIGGER });
                    baseClickInput.IsClicking = true;
                }
                else if (baseClickInput.IsClicking && LeftTriggerSqueezeValue < triggerInput.SqueezeClickThreshold)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.TRIGGER });
                    baseClickInput.IsClicking = true;
                }
                else if (!baseClickInput.IsClicking && !baseClickInput.IsTouching && LeftTriggerSqueezeValue > 0.0f)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.TRIGGER });
                    baseClickInput.IsTouching = true;
                }
                else if (baseClickInput.IsTouching && LeftTriggerSqueezeValue == 0.0f)
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.TRIGGER });
                    baseClickInput.IsTouching = true;
                }
            }
        }
    }
}