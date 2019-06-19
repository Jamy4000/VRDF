using Unity.Entities;
using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Interactions;

namespace VRSF.Utils.Systems
{
    public class ScriptableVariableResetterSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            OnSetupVRReady.Listeners += Init;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= Init;
        }

        private void Init(OnSetupVRReady info)
        {
            ResetInputContainer();
            ResetInteractionContainer();
        }

        private void ResetInputContainer()
        {
            var inputContainer = InputVariableContainer.Instance;

            inputContainer.RightThumbPosition?.SetValue(new Vector2(0, 0));
            inputContainer.LeftThumbPosition?.SetValue(new Vector2(0, 0));
            
            foreach (var kvp in inputContainer.RightClickBoolean.Items)
            {
                kvp.Value?.SetValue(false);
            }

            foreach (var kvp in inputContainer.LeftClickBoolean.Items)
            {
                kvp.Value?.SetValue(false);
            }

            foreach (var kvp in inputContainer.RightTouchBoolean.Items)
            {
                kvp.Value?.SetValue(false);
            }

            foreach (var kvp in inputContainer.LeftTouchBoolean.Items)
            {
                kvp.Value?.SetValue(false);
            }
        }

        private void ResetInteractionContainer()
        {
            var interactionContainer = InteractionVariableContainer.Instance;

            interactionContainer.HasClickSomethingGaze?.SetValue(false);
            interactionContainer.HasClickSomethingLeft?.SetValue(false);
            interactionContainer.HasClickSomethingRight?.SetValue(false);


            interactionContainer.IsOverSomethingGaze?.SetValue(false);
            interactionContainer.IsOverSomethingLeft?.SetValue(false);
            interactionContainer.IsOverSomethingRight?.SetValue(false);


            interactionContainer.RightRay?.SetValue(new Ray());
            interactionContainer.LeftRay?.SetValue(new Ray());
            interactionContainer.GazeRay?.SetValue(new Ray());


            interactionContainer.RightHit?.SetValue(new RaycastHit());
            interactionContainer.RightHit?.SetIsNull(true);

            interactionContainer.LeftHit?.SetValue(new RaycastHit());
            interactionContainer.LeftHit?.SetIsNull(true);

            interactionContainer.GazeHit?.SetValue(new RaycastHit());
            interactionContainer.GazeHit?.SetIsNull(true);

            interactionContainer.PreviousRightHit = null;
            interactionContainer.PreviousLeftHit = null;
            interactionContainer.PreviousGazeHit = null;
        }
    }
}