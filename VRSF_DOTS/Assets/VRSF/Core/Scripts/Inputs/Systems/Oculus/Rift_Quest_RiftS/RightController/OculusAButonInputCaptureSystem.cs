using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture the inputs for the A Button on the right controller of the Oculus Rift, Rift S and QUest
    /// </summary>
    public class OculusAButonInputCaptureSystem : JobComponentSystem
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
            var handle = new AButtonInputCaptureJob()
            {
                AClickButtonDown = Input.GetButtonDown("OculusAButtonClick"),
                AClickButtonUp = Input.GetButtonUp("OculusAButtonClick"),
                ATouchButtonDown = Input.GetButtonDown("OculusAButtonTouch"),
                ATouchButtonUp = Input.GetButtonUp("OculusAButtonTouch"),
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

        [RequireComponentTag(typeof(RightHand), typeof(AButtonInputCapture))]
        struct AButtonInputCaptureJob : IJobForEachWithEntity<BaseInputCapture>
        {
            [ReadOnly] public bool AClickButtonDown;
            [ReadOnly] public bool AClickButtonUp;

            [ReadOnly] public bool ATouchButtonDown;
            [ReadOnly] public bool ATouchButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (AClickButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.A_BUTTON });
                    baseInput.IsClicking = true;
                    baseInput.IsTouching = false;
                }
                else if (AClickButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.A_BUTTON });
                    baseInput.IsClicking = false;
                    baseInput.IsTouching = true;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && !baseInput.IsTouching && ATouchButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.A_BUTTON });
                    baseInput.IsTouching = true;
                }
                else if (baseInput.IsTouching && ATouchButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.A_BUTTON });
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
