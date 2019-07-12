using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the touchpad inputs for the right controller
    /// </summary>
    public class RightTouchpadInputCaptureSystem : JobComponentSystem
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
            return new TouchpadInputCaptureJob()
            {
                RightThumbPosition = new float2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")),
                RightThumbClickDown = Input.GetButtonDown("RightThumbClick"),
                RightThumbClickUp = Input.GetButtonUp("RightThumbClick"),
                RightThumbTouchDown = Input.GetButtonDown("RightThumbTouch"),
                RightThumbTouchUp = Input.GetButtonUp("RightThumbTouch"),
                Commands = _endSimEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [RequireComponentTag(typeof(RightHand))]
        struct TouchpadInputCaptureJob : IJobForEachWithEntity<TouchpadInputCapture, BaseInputCapture>
        {
            [ReadOnly] public float2 RightThumbPosition;

            [ReadOnly] public bool RightThumbClickDown;
            [ReadOnly] public bool RightThumbClickUp;

            [ReadOnly] public bool RightThumbTouchDown;
            [ReadOnly] public bool RightThumbTouchUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref TouchpadInputCapture touchpadInput, ref BaseInputCapture baseInput)
            {
                touchpadInput.ThumbPosition = RightThumbPosition;

                // Check Click Events
                if (RightThumbClickDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsClicking = true;
                }
                else if (RightThumbClickUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsClicking = false;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && RightThumbTouchDown)
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsTouching = true;
                }
                else if (RightThumbTouchUp)
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
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
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
        #endregion PRIVATE_METHODS
    }
}
