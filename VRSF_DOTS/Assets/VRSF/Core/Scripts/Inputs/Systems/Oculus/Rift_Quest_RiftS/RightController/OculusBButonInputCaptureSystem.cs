using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture the inputs for the B Button on the right controller of the Oculus Rift, Rift S and QUest
    /// </summary>
    public class OculusBButonInputCaptureSystem : JobComponentSystem
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
            var handle = new BButtonInputCaptureJob()
            {
                BClickButtonDown = Input.GetButtonDown("OculusBButtonClick"),
                BClickButtonUp = Input.GetButtonUp("OculusBButtonClick"),
                BTouchButtonDown = Input.GetButtonDown("OculusBButtonTouch"),
                BTouchButtonUp = Input.GetButtonUp("OculusBButtonTouch"),
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

        [RequireComponentTag(typeof(RightHand), typeof(BButtonInputCapture))]
        struct BButtonInputCaptureJob : IJobForEachWithEntity<BaseInputCapture>
        {
            [ReadOnly] public bool BClickButtonDown;
            [ReadOnly] public bool BClickButtonUp;

            [ReadOnly] public bool BTouchButtonDown;
            [ReadOnly] public bool BTouchButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (BClickButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.B_BUTTON });
                    baseInput.IsClicking = true;
                    baseInput.IsTouching = false;
                }
                else if (BClickButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.B_BUTTON });
                    baseInput.IsClicking = false;
                    baseInput.IsTouching = true;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && !baseInput.IsTouching && BTouchButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.B_BUTTON });
                    baseInput.IsTouching = true;
                }
                else if (baseInput.IsTouching && BTouchButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.B_BUTTON });
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
            this.Enabled = IsOculusHeadset();

            bool IsOculusHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
            }
        }
        #endregion PRIVATE_METHODS
    }
}