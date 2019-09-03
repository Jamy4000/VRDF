using Unity.Entities;
using VRSF.Core.Events;
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
            Entities.WithAll<LeftHand>().ForEach((ref StopClickingEventComp stopClickingEvent, ref PointerClick pointerClick) =>
            {
                if (stopClickingEvent.ButtonInteracting == pointerClick.ControllersButton)
                {
                    InteractionVariableContainer.IsClickingSomethingLeft = false;
                    //new ObjectWasClickedEvent(Raycast.ERayOrigin.LEFT_HAND, null);
                }
            });

            Entities.WithAll<RightHand>().ForEach((ref StopClickingEventComp stopClickingEvent, ref PointerClick pointerClick) =>
            {
                if (stopClickingEvent.ButtonInteracting == pointerClick.ControllersButton)
                {
                    InteractionVariableContainer.IsClickingSomethingRight = false;
                   // new ObjectWasClickedEvent(Raycast.ERayOrigin.RIGHT_HAND, null);
                }
            });
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= Setup;
            base.OnDestroy();
        }

        private void Setup(OnSetupVRReady info)
        {
            if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
            {
                Entities.ForEach((ref PointerClick pointerClick) =>
                {
                    pointerClick.ControllersButton = EControllersButton.TRIGGER;
                });
            }
        }
    }
}