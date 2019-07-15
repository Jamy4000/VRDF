using Unity.Entities;
using VRSF.Core.Events;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Interactions
{
    /// <summary>
    /// Handle the Unclick event in VR. Basically link the Raycast system and the Input System.
    /// </summary>
    public class PointerUnclickingSystem : ComponentSystem
    {
        #region ComponentSystem_Methods
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
                    InteractionVariableContainer.HasClickSomethingLeft = false;
            });

            Entities.ForEach((ref StopClickingEventComp stopClickingEvent, ref RightHand rightHand) =>
            {
                if (stopClickingEvent.ButtonInteracting == EControllersButton.TRIGGER)
                    InteractionVariableContainer.HasClickSomethingRight = false;
            });
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= Setup;
            base.OnDestroy();
        }
        #endregion


        #region PRIVATE_METHODS
        private void Setup(OnSetupVRReady info)
        {
            this.Enabled = GetEntityQuery(typeof(PointerClick)).CalculateLength() > 0;
        }
        #endregion PRIVATE_METHODS
    }
}