using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the touchpad inputs for the left controller
    /// </summary>
    public class TouchpadInputCaptureSystem : ComponentSystem
    {
        private JobHandle _inputDeps;

        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (_inputDeps.IsCompleted)
            {
                TouchpadInputCaptureJob job = new TouchpadInputCaptureJob()
                {
                    LeftThumbPosition = new float2(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft")),
                    LeftThumbClickDown = Input.GetButtonDown("LeftThumbClick"),
                    LeftThumbClickUp = Input.GetButtonUp("LeftThumbClick"),
                    LeftThumbTouchDown = Input.GetButtonDown("LeftThumbTouch"),
                    LeftThumbTouchUp = Input.GetButtonUp("LeftThumbTouch"),

                    RightThumbPosition = new float2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")),
                    RightThumbClickDown = Input.GetButtonDown("RightThumbClick"),
                    RightThumbClickUp = Input.GetButtonUp("RightThumbClick"),
                    RightThumbTouchDown = Input.GetButtonDown("RightThumbTouch"),
                    RightThumbTouchUp = Input.GetButtonUp("RightThumbTouch"),

                    ShouldRaiseLeftEvent = new NativeArray<bool>(4, Allocator.TempJob),
                    ShouldRaiseRightEvent = new NativeArray<bool>(4, Allocator.TempJob)
                };

                _inputDeps = job.Schedule(this);
                _inputDeps.Complete();

                CheckEventToRaise(job.ShouldRaiseLeftEvent, EHand.LEFT);
                CheckEventToRaise(job.ShouldRaiseRightEvent, EHand.RIGHT);

                job.ShouldRaiseLeftEvent.Dispose();
                job.ShouldRaiseRightEvent.Dispose();
            }
        }

        private void CheckEventToRaise(NativeArray<bool> shouldRaiseEventList, EHand hand)
        {
            if (shouldRaiseEventList[0])
                new ButtonClickEvent(hand, EControllersButton.TOUCHPAD);
            else if (shouldRaiseEventList[1])
                new ButtonUnclickEvent(hand, EControllersButton.TOUCHPAD);
            else if (shouldRaiseEventList[2])
                new ButtonTouchEvent(hand, EControllersButton.TOUCHPAD);
            else if (shouldRaiseEventList[3])
                new ButtonUntouchEvent(hand, EControllersButton.TOUCHPAD);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct TouchpadInputCaptureJob : IJobForEach<TouchpadInputCapture, BaseInputCapture>
        {
            public float2 LeftThumbPosition;
            public float2 RightThumbPosition;

            public bool LeftThumbClickDown;
            public bool LeftThumbClickUp;
            public bool RightThumbClickDown;
            public bool RightThumbClickUp;

            public bool LeftThumbTouchDown;
            public bool LeftThumbTouchUp;
            public bool RightThumbTouchDown;
            public bool RightThumbTouchUp;

            // Outputs Array, way of doing it in Unity Jobs
            public NativeArray<bool> ShouldRaiseLeftEvent;
            public NativeArray<bool> ShouldRaiseRightEvent;

            public void Execute(ref TouchpadInputCapture touchpadInput, ref BaseInputCapture baseInput)
            {
                // This system only works for the left controller, as the left input are given as parameters of this system
                if (touchpadInput.Hand == EHand.LEFT)
                {
                    touchpadInput.ThumbPosition = LeftThumbPosition;
                    CheckInputs(LeftThumbClickDown, LeftThumbClickUp, LeftThumbTouchDown, LeftThumbTouchUp, ref baseInput, ref ShouldRaiseLeftEvent);
                }
                else
                {
                    touchpadInput.ThumbPosition = RightThumbPosition;
                    CheckInputs(LeftThumbClickDown, LeftThumbClickUp, LeftThumbTouchDown, LeftThumbTouchUp, ref baseInput, ref ShouldRaiseLeftEvent);
                }
            }

            private void CheckInputs(bool thumbClickDown, bool thumbClickUp, bool thumbTouchDown, bool thumbTouchUp, ref BaseInputCapture baseInput, ref NativeArray<bool> shouldRaiseEvent)
            {
                // Check Click Events
                if (thumbClickDown)
                {
                    baseInput.IsClicking = true;
                    baseInput.IsTouching = false;
                    shouldRaiseEvent[0] = true;
                }
                else if (thumbClickUp)
                {
                    baseInput.IsClicking = false;
                    shouldRaiseEvent[1] = true;
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && thumbTouchDown)
                {
                    baseInput.IsTouching = true;
                    shouldRaiseEvent[2] = true;
                }
                else if (thumbTouchUp)
                {
                    baseInput.IsTouching = false;
                    shouldRaiseEvent[3] = true;
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
