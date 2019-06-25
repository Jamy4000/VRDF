using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusLeftThumbrestInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckForComponents;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new ThumbrestButtonInputCaptureJob()
            {
                ThumbrestTouchButtonDown = Input.GetButtonDown("OculusLeftThumbrestTouch"),
                ThumbrestTouchButtonUp = Input.GetButtonUp("OculusLeftThumbrestTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForComponents;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct ThumbrestButtonInputCaptureJob : IJobForEach<ThumbrestInputCapture>
        {
            public bool ThumbrestTouchButtonDown;
            public bool ThumbrestTouchButtonUp;

            public void Execute(ref ThumbrestInputCapture thumbrestCapture)
            {
                if (thumbrestCapture.Hand == EHand.LEFT)
                {
                    if (ThumbrestTouchButtonDown)
                    {
                        thumbrestCapture.ThumbrestTouch = true;
                        new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
                    }
                    else if (ThumbrestTouchButtonUp)
                    {
                        thumbrestCapture.ThumbrestTouch = false;
                        new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's at least one ThumbrestInputCapture component and that it has the LEFT as Hand
        /// </summary>
        /// <param name="info"></param>
        private void CheckForComponents(OnSetupVRReady info)
        {
            if (IsOculusHeadset())
            {
                var entityQuery = GetEntityQuery(typeof(ThumbrestInputCapture)).ToComponentDataArray<ThumbrestInputCapture>(Unity.Collections.Allocator.TempJob, out JobHandle jobHandle);
                if (entityQuery.Length > 0)
                {
                    foreach (var tic in entityQuery)
                    {
                        if (tic.Hand == EHand.LEFT)
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


            bool IsOculusHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
            }
        }
        #endregion PRIVATE_METHODS
    }
}