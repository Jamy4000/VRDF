using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the right controllers
    /// </summary>
    public class RightGripInputCaptureSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

        protected override void OnCreate()
        {
            // Cache the EndSimulationEntityCommandBufferSystem in a field, so we don't have to get it every frame
            _endSimEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = new GripInputCaptureJob
            {
                RightGripSqueezeValue = Input.GetAxis("RightGripSqueeze"),
                Commands = _endSimEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            handle.Complete();
            return handle;
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [RequireComponentTag(typeof(RightHand))]
        struct GripInputCaptureJob : IJobForEachWithEntity<GripInputCapture, BaseInputCapture>
        {
            [ReadOnly] public float RightGripSqueezeValue;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref GripInputCapture gripInput, ref BaseInputCapture baseInput)
            {
                if (!baseInput.IsClicking && RightGripSqueezeValue > gripInput.SqueezeClickThreshold)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.GRIP });
                    baseInput.IsTouching = false;
                    baseInput.IsClicking = true;
                }
                else if (baseInput.IsClicking && RightGripSqueezeValue < gripInput.SqueezeClickThreshold)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.GRIP });
                    baseInput.IsClicking = false;
                    baseInput.IsTouching = true;
                }
                else if (!baseInput.IsClicking && !baseInput.IsTouching && RightGripSqueezeValue > 0.0f)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.GRIP });
                    baseInput.IsTouching = true;
                }
                else if (baseInput.IsTouching && RightGripSqueezeValue == 0.0f)
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.GRIP });
                    baseInput.IsTouching = false;
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
