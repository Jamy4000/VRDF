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
        private bool _isUsingOculusDevice;

        protected override void OnCreate()
        {
            // Cache the EndSimulationEntityCommandBufferSystem in a field, so we don't have to get it every frame
            _endSimEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = new TouchpadInputCaptureJob()
            {
                RightThumbPosition = new float2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")),
                RightThumbClickDown = Input.GetButtonDown("RightThumbClick"),
                RightThumbClickUp = Input.GetButtonUp("RightThumbClick"),
                RightThumbTouchDown = Input.GetButtonDown("RightThumbTouch"),
                RightThumbTouchUp = Input.GetButtonUp("RightThumbTouch"),
                IsUsingOculusDevice = _isUsingOculusDevice,
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
        struct TouchpadInputCaptureJob : IJobForEachWithEntity<TouchpadInputCapture, BaseInputCapture>
        {
            [ReadOnly] public float2 RightThumbPosition;

            [ReadOnly] public bool RightThumbClickDown;
            [ReadOnly] public bool RightThumbClickUp;

            [ReadOnly] public bool RightThumbTouchDown;
            [ReadOnly] public bool RightThumbTouchUp;

            [ReadOnly] public bool IsUsingOculusDevice;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref TouchpadInputCapture touchpadInput, ref BaseInputCapture baseInput)
            {
                touchpadInput.ThumbPosition = RightThumbPosition;

                // Check Click Events
                if (RightThumbClickDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsTouching = false;
                    baseInput.IsClicking = true;
                }
                else if (RightThumbClickUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsClicking = false;
                    baseInput.IsTouching = true;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && !baseInput.IsTouching && UserIsTouching(touchpadInput.UseThumbPosForTouch))
                {
                    Commands.AddComponent(index, entity, new StartTouchingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsTouching = true;
                }
                else if (baseInput.IsTouching && UserStopTouching(touchpadInput.UseThumbPosForTouch))
                {
                    Commands.AddComponent(index, entity, new StopTouchingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsTouching = false;
                }
            }

            private bool UserStopTouching(bool useThumbPosForTouch)
            {
                return useThumbPosForTouch && IsUsingOculusDevice ? !UserIsMovingThumbstick() : RightThumbTouchUp;
            }

            private bool UserIsTouching(bool useThumbPosForTouch)
            {
                return useThumbPosForTouch && IsUsingOculusDevice ? UserIsMovingThumbstick() : RightThumbTouchDown;
            }

            private bool UserIsMovingThumbstick()
            {
                return RightThumbPosition.x != 0.0f || RightThumbPosition.y != 0.0f;
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
            _isUsingOculusDevice = VRSF_Components.DeviceLoaded == EDevice.OCULUS_GO || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
        }
        #endregion PRIVATE_METHODS
    }
}
