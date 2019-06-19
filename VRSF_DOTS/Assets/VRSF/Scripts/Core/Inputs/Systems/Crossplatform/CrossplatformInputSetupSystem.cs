using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the Simulator, Vive and Rift, capture the basic inputs.
    /// </summary>
    public class CrossplatformInputSetupSystem : ComponentSystem
    {
        private struct Filter
        {
            public CrossplatformInputCapture InputCapture;
        }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            OnSetupVRReady.Listeners += SetupControllersParameters;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= SetupControllersParameters;
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Setup the two controllers parameters to use in the CheckControllersInput method.
        /// </summary>
        public void SetupControllersParameters(OnSetupVRReady info)
        {
            foreach (var e in GetEntities<Filter>())
            {
                // We give the references to the Scriptable variable containers in the Left Parameters variable
                e.InputCapture.LeftParameters = new InputParameters
                (
                    InputVariableContainer.Instance.LeftClickBoolean,
                    InputVariableContainer.Instance.LeftTouchBoolean,
                    InputVariableContainer.Instance.LeftThumbPosition,
                    InputVariableContainer.Instance.LeftTriggerSqueezeValue,
                    InputVariableContainer.Instance.LeftGripSqueezeValue
                );

                // We give the references to the Scriptable variable containers in the Right Parameters variable
                e.InputCapture.RightParameters = new InputParameters
                (
                    InputVariableContainer.Instance.RightClickBoolean,
                    InputVariableContainer.Instance.RightTouchBoolean,
                    InputVariableContainer.Instance.RightThumbPosition,
                    InputVariableContainer.Instance.RightTriggerSqueezeValue,
                    InputVariableContainer.Instance.RightGripSqueezeValue
                );

                e.InputCapture.IsSetup = true;
                new OnCrossplatformComponentIsSetup();
            }
        }
        #endregion PRIVATE_METHODS
    }
}
