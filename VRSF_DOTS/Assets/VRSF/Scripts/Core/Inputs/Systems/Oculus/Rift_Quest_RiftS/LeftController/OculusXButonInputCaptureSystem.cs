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
            OnSetupVRReady.Listeners += CheckDevice;
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
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct XButtonInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public bool XClickButtonDown;
            public bool XClickButtonUp;

            public bool XTouchButtonDown;
            public bool XTouchButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (XClickButtonDown)
                {
                    LeftInputsParameters.X_Click = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
                else if (XClickButtonUp)
                {
                    LeftInputsParameters.X_Click = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!LeftInputsParameters.X_Click && XTouchButtonDown)
                {
                    LeftInputsParameters.X_Touch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
                else if (XTouchButtonUp)
                {
                    LeftInputsParameters.X_Touch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
                }
            }
        }

        #region PRIVATE_METHODS
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = IsOculusHeadset();

            bool IsOculusHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
