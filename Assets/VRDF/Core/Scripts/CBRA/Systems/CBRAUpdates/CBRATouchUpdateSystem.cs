using Unity.Entities;
using VRDF.Core.Inputs;

namespace VRDF.Core.CBRA
{
    /// <summary>
    /// Handle the touch on all controllers button EXCEPT the touchpad
    /// </summary>
    public class CBRATouchUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            // Not checking the TouchpadInputCapture as it has its own dedicated system
            Entities.WithAll<CBRATag>().WithNone<TouchpadInputCapture, StartTouchingEventComp, StopTouchingEventComp>().ForEach((Entity entity, ref BaseInputCapture baseInput) =>
            {
                if (baseInput.IsTouching && CBRADelegatesHolder.IsTouchingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });
        }
    }
}