﻿using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRDF.Core.SetupVR;

namespace VRDF.Core.Inputs
{
    /// <summary>
    /// Capture inputs for the menu buttons of the HTC Vive and Focus for the right controller
    /// </summary>
    public class HtcRightMenuInputCaptureSystem : JobComponentSystem
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
            var handle = new MenuInputCaptureJob
            {
                RightMenuButtonDown = Input.GetButtonDown("HtcRightMenuClick"),
                RightMenuButtonUp = Input.GetButtonUp("HtcRightMenuClick"),
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
        struct MenuInputCaptureJob : IJobForEachWithEntity<MenuInputCapture, BaseInputCapture>
        {
            [ReadOnly] public bool RightMenuButtonDown;
            [ReadOnly] public bool RightMenuButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref MenuInputCapture menuInput, ref BaseInputCapture baseInput)
            {
                if (RightMenuButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = EControllersButton.MENU });
                    baseInput.IsClicking = true;
                }
                else if (RightMenuButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = EControllersButton.MENU });
                    baseInput.IsClicking = false;
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
            this.Enabled = VRDF_Components.DeviceLoaded == EDevice.HTC_FOCUS || VRDF_Components.DeviceLoaded == EDevice.HTC_VIVE;
        }
        #endregion PRIVATE_METHODS
    }
}