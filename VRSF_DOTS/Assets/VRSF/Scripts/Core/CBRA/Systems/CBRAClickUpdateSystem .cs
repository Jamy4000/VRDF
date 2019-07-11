using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the clicks on all controllers button EXCEPT the touchpad
    /// </summary>
    public class CBRAClickUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType, ref BaseInputCapture baseInput) =>
            {
                if (CBRADelegatesHolder.ClickEvents.ContainsKey(entity) && baseInput.IsClicking)
                    CBRADelegatesHolder.ClickEvents[entity][ActionType.IsInteracting].Invoke();
            });
        }
    }
}