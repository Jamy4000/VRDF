using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the touchpad inputs for the left controller
    /// </summary>
    public class LeftTouchpadInputCaptureSystem : JobComponentSystem
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
                LeftThumbPosition = new float2(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft")),
                LeftThumbClickDown = Input.GetButtonDown("LeftThumbClick"),
                LeftThumbClickUp = Input.GetButtonUp("LeftThumbClick"),
                LeftThumbTouchDown = Input.GetButtonDown("LeftThumbTouch"),
                LeftThumbTouchUp = Input.GetButtonUp("LeftThumbTouch"),
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

        [RequireComponentTag(typeof(LeftHand))]
        struct TouchpadInputCaptureJob : IJobForEachWithEntity<TouchpadInputCapture, BaseInputCapture>
        {
            [ReadOnly] public float2 LeftThumbPosition;

            [ReadOnly] public bool LeftThumbClickDown;
            [ReadOnly] public bool LeftThumbClickUp;

            [ReadOnly] public bool LeftThumbTouchDown;
            [ReadOnly] public bool LeftThumbTouchUp;

            [ReadOnly] public bool IsUsingOculusDevice;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref TouchpadInputCapture touchpadInput, ref BaseInputCapture baseInput)
            {
                touchpadInput.ThumbPosition = LeftThumbPosition;

                // Check Click Events
                if (LeftThumbClickDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.TOUCHPAD });
                    baseInput.IsTouching = false;
                    baseInput.IsClicking = true;
                }
                else if (LeftThumbClickUp)
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
                return useThumbPosForTouch && IsUsingOculusDevice ? !UserIsMovingThumbstick() : LeftThumbTouchUp;
            }

            private bool UserIsTouching(bool useThumbPosForTouch)
            {
                return useThumbPosForTouch && IsUsingOculusDevice ? UserIsMovingThumbstick() : LeftThumbTouchDown;
            }

            private bool UserIsMovingThumbstick()
            {
                return LeftThumbPosition.x != 0.0f || LeftThumbPosition.y != 0.0f;
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check which device we're using
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