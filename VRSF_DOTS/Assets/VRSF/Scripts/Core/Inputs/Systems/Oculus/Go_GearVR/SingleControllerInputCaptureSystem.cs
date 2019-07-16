using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Only capturing the Back Button on the GearVR and the Oculus Go controller
    /// </summary>
    public class SignleControllerInputCaptureSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

        protected override void OnCreate()
        {
            // Cache the EndSimulationEntityCommandBufferSystem in a field, so we don't have to get it every frame
            _endSimEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            OnSetupVRReady.Listeners += CheckForDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = new BackButtonInputCaptureJob()
            {
                MenuButtonDown = Input.GetButtonDown("BackButtonClick"),
                MenuButtonUp = Input.GetButtonUp("BackButtonClick"),
                Commands = _endSimEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            handle.Complete();
            return handle;
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForDevice;
            base.OnDestroy();
        }

        struct BackButtonInputCaptureJob : IJobForEachWithEntity<GoAndGearVRInputCapture, BaseInputCapture>
        {
            [ReadOnly] public bool MenuButtonDown;
            [ReadOnly] public bool MenuButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref GoAndGearVRInputCapture goAndGearInput, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (MenuButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.BACK_BUTTON });
                    baseInput.IsClicking = true;
                }
                else if (MenuButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.BACK_BUTTON });
                    baseInput.IsClicking = false;
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if we use the good device
        /// </summary>
        /// <param name="info"></param>
        private void CheckForDevice(OnSetupVRReady info)
        {
            this.Enabled = IsSingleControllerHeadset();

            bool IsSingleControllerHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.GEAR_VR || VRSF_Components.DeviceLoaded == EDevice.OCULUS_GO;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
