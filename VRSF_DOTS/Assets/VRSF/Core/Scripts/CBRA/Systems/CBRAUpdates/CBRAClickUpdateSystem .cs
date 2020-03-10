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
            // Not checking the TouchpadInputCapture as it has its own dedicated system
            Entities.WithAll<CBRAEventComponent>().WithNone<TouchpadInputCapture, StartClickingEventComp, StopClickingEventComp>().ForEach((Entity entity, ref BaseInputCapture baseInput) =>
            {
                if (baseInput.IsClicking && CBRADelegatesHolder.IsClickingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });
        }
    }
}