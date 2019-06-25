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
    public class OculusXButonInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckForComponents;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new XButtonInputCaptureJob()
            {
                XClickButtonDown = Input.GetButtonDown("OculusXButtonClick"),
                XClickButtonUp = Input.GetButtonUp("OculusXButtonClick"),
                XTouchButtonDown = Input.GetButtonDown("OculusXButtonTouch"),
                XTouchButtonUp = Input.GetButtonUp("OculusXButtonTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForComponents;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct XButtonInputCaptureJob : IJobForEach<XButtonInputCapture>
        {
            public bool XClickButtonDown;
            public bool XClickButtonUp;

            public bool XTouchButtonDown;
            public bool XTouchButtonUp;

            public void Execute(ref XButtonInputCapture xButtonInput)
            {
                // Check Click Events
                if (XClickButtonDown)
                {
                    xButtonInput.X_Click = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
                else if (XClickButtonUp)
                {
                    xButtonInput.X_Click = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!xButtonInput.X_Click && XTouchButtonDown)
                {
                    xButtonInput.X_Touch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
                else if (XTouchButtonUp)
                {
                    xButtonInput.X_Touch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if there's at least one AButtonInputCapture component and that it has the RIGHT as Hand
        /// </summary>
        /// <param name="info"></param>
        private void CheckForComponents(OnSetupVRReady info)
        {
            this.Enabled = IsOculusHeadset() && GetEntityQuery(typeof(XButtonInputCapture)).CalculateLength() > 0;

            bool IsOculusHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
