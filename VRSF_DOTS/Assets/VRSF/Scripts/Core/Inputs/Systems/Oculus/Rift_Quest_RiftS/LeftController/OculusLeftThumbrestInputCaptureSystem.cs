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
    public class OculusLeftThumbrestInputCaptureSystem : JobComponentSystem
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
            var handle = new ThumbrestButtonInputCaptureJob()
            {
                ThumbrestTouchButtonDown = Input.GetButtonDown("OculusLeftThumbrestTouch"),
                ThumbrestTouchButtonUp = Input.GetButtonUp("OculusLeftThumbrestTouch"),
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
        struct ThumbrestButtonInputCaptureJob : IJobForEachWithEntity<ThumbrestInputCapture, BaseInputCapture>
        {
            [ReadOnly] public bool ThumbrestTouchButtonDown;
            [ReadOnly] public bool ThumbrestTouchButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref ThumbrestInputCapture thumbrestCapture, ref BaseInputCapture baseInput)
            {
                if (ThumbrestTouchButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.THUMBREST });
                    baseInput.IsTouching = true;
                }
                else if (ThumbrestTouchButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.THUMBREST });
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