using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Handle the Unclick event in VR. Basically link the Raycast system and the Input System.
    /// </summary>
    public class PointerUnclickingSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<LeftHand>().ForEach((ref StopClickingEventComp stopClickingEvent, ref PointerClicker pointerClick) =>
            {
                if (stopClickingEvent.ButtonInteracting == pointerClick.ControllersButton)
                    VRInteractions.InteractionVariableContainer.IsClickingSomethingLeft = false;
            });

            Entities.WithAll<RightHand>().ForEach((ref StopClickingEventComp stopClickingEvent, ref PointerClicker pointerClick) =>
            {
                if (stopClickingEvent.ButtonInteracting == pointerClick.ControllersButton)
                    VRInteractions.InteractionVariableContainer.IsClickingSomethingRight = false;
            });
        }
    }
}