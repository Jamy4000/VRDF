using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// Handle the Unclick event in VR. Basically link the Raycast system and the Input System.
    /// </summary>
    public class PointerUnclickingSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += Setup;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref StopClickingEventComp stopClickingEvent, ref LeftHand leftHand) =>
            {
                if (stopClickingEvent.ButtonInteracting == EControllersButton.TRIGGER)
                    InteractionVariableContainer.IsClickingSomethingLeft = false;
            });

            Entities.ForEach((ref StopClickingEventComp stopClickingEvent, ref RightHand rightHand) =>
            {
                if (stopClickingEvent.ButtonInteracting == EControllersButton.TRIGGER)
                    InteractionVariableContainer.IsClickingSomethingRight = false;
            });
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= Setup;
            base.OnDestroy();
        }

        private void Setup(OnSetupVRReady info)
        {
            this.Enabled = GetEntityQuery(typeof(PointerClick)).CalculateLength() > 0;
        }
    }
}