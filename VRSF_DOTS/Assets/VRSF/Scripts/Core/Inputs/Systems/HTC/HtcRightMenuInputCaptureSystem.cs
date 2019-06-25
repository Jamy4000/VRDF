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
    public class HtcRightMenuInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckForComponents;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new MenuInputCaptureJob()
            {
                MenuButtonDown = Input.GetButtonDown("HtcRightMenuClick"),
                MenuButtonUp = Input.GetButtonUp("HtcRightMenuClick")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForComponents;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct MenuInputCaptureJob : IJobForEach<MenuInputCapture>
        {
            public bool MenuButtonDown;
            public bool MenuButtonUp;

            public void Execute(ref MenuInputCapture menuInput)
            {
                // This system only works for the right controller, as the right input are given as parameters of this system
                if (menuInput.Hand == EHand.RIGHT)
                {
                    // Check Click Events
                    if (MenuButtonDown)
                    {
                        menuInput.MenuClick = true;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
                    }
                    else if (MenuButtonUp)
                    {
                        menuInput.MenuClick = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's at least one MenuInputCapture component and that it has the RIGHT as Hand
        /// </summary>
        /// <param name="info"></param>
        private void CheckForComponents(OnSetupVRReady info)
        {
            if (VRSF_Components.DeviceLoaded == EDevice.HTC_FOCUS || VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE)
            {
                var entityQuery = GetEntityQuery(typeof(MenuInputCapture)).ToComponentDataArray<MenuInputCapture>(Unity.Collections.Allocator.TempJob, out JobHandle jobHandle);
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
            }
            this.Enabled = false;
        }
        #endregion PRIVATE_METHODS
    }
}