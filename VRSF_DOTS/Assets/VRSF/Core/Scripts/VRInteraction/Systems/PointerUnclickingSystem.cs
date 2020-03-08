using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// Handle the Unclick event in VR. Basically link the Raycast system and the Input System.
    /// </summary>
    public class PointerUnclickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<LeftHand>().ForEach((ref StopClickingEventComp stopClickingEvent, ref PointerClick pointerClick) =>
            {
                if (stopClickingEvent.ButtonInteracting == pointerClick.ControllersButton)
                    InteractionVariableContainer.IsClickingSomethingLeft = false;
            });

            Entities.WithAll<RightHand>().ForEach((ref StopClickingEventComp stopClickingEvent, ref PointerClick pointerClick) =>
            {
                if (stopClickingEvent.ButtonInteracting == pointerClick.ControllersButton)
                    InteractionVariableContainer.IsClickingSomethingRight = false;
            });
        }
    }
}