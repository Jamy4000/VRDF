using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the left controllers
    /// </summary>
    public class LeftGripInputCaptureSystem : JobComponentSystem
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
                LeftGripSqueezeValue = Input.GetAxis("LeftGripSqueeze"),
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

        [RequireComponentTag(typeof(LeftHand))]
        struct GripInputCaptureJob : IJobForEachWithEntity<GripInputCapture, BaseInputCapture>
        {
            [ReadOnly] public float LeftGripSqueezeValue;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref GripInputCapture gripInput, ref BaseInputCapture baseInput)
            {
                if (!baseInput.IsClicking && LeftGripSqueezeValue > gripInput.SqueezeClickThreshold)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.GRIP });
                    baseInput.IsTouching = false;
                    baseInput.IsClicking = true;
                }
                else if (baseInput.IsClicking && LeftGripSqueezeValue < gripInput.SqueezeClickThreshold)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.GRIP });
                    baseInput.IsClicking = false;
                    baseInput.IsTouching = true;
                }
                else if (!baseInput.IsClicking && !baseInput.IsTouching && LeftGripSqueezeValue > 0.0f)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.GRIP });
                    baseInput.IsTouching = true;
                }
                else if (baseInput.IsTouching && LeftGripSqueezeValue == 0.0f)
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
