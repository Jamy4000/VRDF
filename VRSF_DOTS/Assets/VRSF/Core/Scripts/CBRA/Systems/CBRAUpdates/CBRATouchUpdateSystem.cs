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
            // Not checking the TouchpadInputCapture as it has its own dedicated system
            Entities.WithAll<CBRAEventComponent>().WithNone<TouchpadInputCapture>().ForEach((Entity entity, ref BaseInputCapture baseInput) =>
            {
                if (baseInput.IsTouching && CBRADelegatesHolder.IsTouchingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });
        }
    }
}