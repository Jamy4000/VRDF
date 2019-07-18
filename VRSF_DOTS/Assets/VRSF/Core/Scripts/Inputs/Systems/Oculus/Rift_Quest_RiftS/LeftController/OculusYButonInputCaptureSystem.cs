using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusYButonInputCaptureSystem : JobComponentSystem
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
            var handle = new YButtonInputCaptureJob()
            {
                YClickButtonDown = Input.GetButtonDown("OculusYButtonClick"),
                YClickButtonUp = Input.GetButtonUp("OculusYButtonClick"),
                YTouchButtonDown = Input.GetButtonDown("OculusYButtonTouch"),
                YTouchButtonUp = Input.GetButtonUp("OculusYButtonTouch"),
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

        [RequireComponentTag(typeof(LeftHand), typeof(YButtonInputCapture))]
        struct YButtonInputCaptureJob : IJobForEachWithEntity<BaseInputCapture>
        {
            [ReadOnly] public bool YClickButtonDown;
            [ReadOnly] public bool YClickButtonUp;

            [ReadOnly] public bool YTouchButtonDown;
            [ReadOnly] public bool YTouchButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (YClickButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.Y_BUTTON });
                    baseInput.IsClicking = true;
                    baseInput.IsTouching = false;
                }
                else if (YClickButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.Y_BUTTON });
                    baseInput.IsClicking = false;
                    baseInput.IsTouching = true;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && !baseInput.IsTouching && YTouchButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.Y_BUTTON });
                    baseInput.IsTouching = true;
                }
                else if (baseInput.IsTouching && YTouchButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.Y_BUTTON });
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