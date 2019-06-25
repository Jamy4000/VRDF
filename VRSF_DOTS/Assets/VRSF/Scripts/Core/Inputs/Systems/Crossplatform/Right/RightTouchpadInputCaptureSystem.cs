using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the touchpad inputs for the right controller
    /// </summary>
    public class RightTouchpadInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckForComponents;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TouchpadInputCaptureJob()
            {
                ThumbPosition = new float2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")),
                RightThumbClickDown = Input.GetButtonDown("RightThumbClick"),
                RightThumbClickUp = Input.GetButtonUp("RightThumbClick"),
                RightThumbTouchDown = Input.GetButtonDown("RightThumbTouch"),
                RightThumbTouchUp = Input.GetButtonUp("RightThumbTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForComponents;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct TouchpadInputCaptureJob : IJobForEach<TouchpadInputCapture>
        {
            public float2 ThumbPosition;

            public bool RightThumbClickDown;
            public bool RightThumbClickUp;

            public bool RightThumbTouchDown;
            public bool RightThumbTouchUp;

            public void Execute(ref TouchpadInputCapture touchpadInput)
            {
                // This system only works for the right controller, as the right input are given as parameters of this system
                if (touchpadInput.Hand == EHand.RIGHT)
                {
                    touchpadInput.ThumbPosition = ThumbPosition;

                    // Check Click Events
                    if (RightThumbClickDown)
                    {
                        touchpadInput.TouchpadClick = true;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
                    }
                    else if (RightThumbClickUp)
                    {
                        touchpadInput.TouchpadClick = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!touchpadInput.TouchpadClick && RightThumbTouchDown)
                    {
                        touchpadInput.TouchpadTouch = true;
                        new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
                    }
                    else if (RightThumbTouchUp)
                    {
                        touchpadInput.TouchpadTouch = false;
                        new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's at least one TOuchpadInputCapture component and that it has the RIGHT as Hand
        /// </summary>
        /// <param name="info"></param>
        private void CheckForComponents(OnSetupVRReady info)
        {
            var entityQuery = GetEntityQuery(typeof(TouchpadInputCapture)).ToComponentDataArray<TouchpadInputCapture>(Unity.Collections.Allocator.TempJob, out JobHandle jobHandle);
            if (entityQuery.Length > 0)
            {
                foreach (var tic in entityQuery)
                {
                    if (tic.Hand == EHand.RIGHT)
                    {
                        this.Enabled = true;
                        jobHandle.Complete();
                        entityQuery.Dispose();
                        return;
                    }
                }
            }
            jobHandle.Complete();
            entityQuery.Dispose();
            this.Enabled = false;
        }
        #endregion PRIVATE_METHODS
    }
}
