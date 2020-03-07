using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System to capture some keys from the simulator
    /// </summary>
    public class SimulatorLeftMouseButtonInputCaptureSystem : JobComponentSystem
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
            var handle = new SimulatorInputCaptureJob()
            {
                LeftMouseButtonDown = Input.GetMouseButtonDown(0),
                LeftMouseButtonUp = Input.GetMouseButtonUp(0),
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
        struct SimulatorInputCaptureJob : IJobForEachWithEntity<TriggerInputCapture, BaseInputCapture>
        {
            [ReadOnly] public bool LeftMouseButtonDown;
            [ReadOnly] public bool LeftMouseButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref TriggerInputCapture triggerInput, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (LeftMouseButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.TRIGGER });
                    baseInput.IsClicking = true;
                }
                else if (LeftMouseButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.TRIGGER });
                    baseInput.IsClicking = false;
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
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}