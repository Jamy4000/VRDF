using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the touch on all controllers button EXCEPT the touchpad
    /// </summary>
    public class CBRATouchUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CBRAInteractionType cbraInteractionType, ref BaseInputCapture baseInput) =>
            {
                if (CBRADelegatesHolder.TouchEvents.ContainsKey(entity) && baseInput.IsTouching)
                    CBRADelegatesHolder.TouchEvents[entity][ActionType.IsInteracting].Invoke();
            });
        }
    }
}