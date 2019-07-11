using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture inputs for the menu buttons of the HTC Vive and Focus for the right controller
    /// </summary>
    public class HtcMenuInputCaptureSystem : ComponentSystem
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
                MenuInputCaptureJob job = new MenuInputCaptureJob
                {
                    RightMenuButtonDown = Input.GetButtonDown("HtcRightMenuClick"),
                    RightMenuButtonUp = Input.GetButtonUp("HtcRightMenuClick"),
                    LeftMenuButtonDown = Input.GetButtonDown("HtcLeftMenuClick"),
                    LeftMenuButtonUp = Input.GetButtonUp("HtcLeftMenuClick"),
                    ShouldRaiseLeftEvent = new NativeArray<bool>(2, Allocator.TempJob),
                    ShouldRaiseRightEvent = new NativeArray<bool>(2, Allocator.TempJob)
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
                new ButtonClickEvent(hand, EControllersButton.MENU);
            else if (shouldRaiseEventList[1])
                new ButtonUnclickEvent(hand, EControllersButton.MENU);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct MenuInputCaptureJob : IJobForEach<MenuInputCapture, BaseInputCapture>
        {
            public bool RightMenuButtonDown;
            public bool RightMenuButtonUp;
            public bool LeftMenuButtonDown;
            public bool LeftMenuButtonUp;

            // Outputs Array, way of doing it in Unity Jobs
            public NativeArray<bool> ShouldRaiseLeftEvent;
            public NativeArray<bool> ShouldRaiseRightEvent;

            public void Execute(ref MenuInputCapture menuInput, ref BaseInputCapture baseInput)
            {
                bool menuDown;
                bool menuUp;
                NativeArray<bool> outputs;

                if (menuInput.Hand == EHand.LEFT)
                {
                    menuDown = LeftMenuButtonDown;
                    menuUp = LeftMenuButtonUp;
                    outputs = ShouldRaiseLeftEvent;
                }
                else
                {
                    menuDown = RightMenuButtonDown;
                    menuUp = RightMenuButtonUp;
                    outputs = ShouldRaiseRightEvent;
                }

                // Check Click Events
                if (menuDown)
                {
                    baseInput.IsClicking = true;
                    outputs[0] = true;
                }
                else if (menuUp)
                {
                    baseInput.IsClicking = false;
                    outputs[1] = true;
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
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.HTC_FOCUS || VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE;
        }
        #endregion PRIVATE_METHODS
    }
}