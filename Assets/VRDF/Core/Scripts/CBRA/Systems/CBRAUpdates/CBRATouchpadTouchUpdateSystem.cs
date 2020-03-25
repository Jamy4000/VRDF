using Unity.Entities;
using VRDF.Core.Inputs;
using VRDF.Core.VRInteractions;

namespace VRDF.Core.CBRA
{
    /// <summary>
    /// Handle the touch on the touchpads ONLY
    /// </summary>
    public class CBRATouchpadTouchUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            // Checking only the TouchpadInputCapture components as the others have their own dedicated system
            Entities.WithNone<StartTouchingEventComp, StopTouchingEventComp>().ForEach((Entity entity, ref TouchpadInputCapture touchpadInput, ref InteractionThumbPosition cbraThumbPos, ref BaseInputCapture baseInput) => 
            {
                if (baseInput.IsTouching && ThumbPositionChecker.CheckThumbPosition(cbraThumbPos.TouchThumbPosition, cbraThumbPos.IsTouchingThreshold, touchpadInput.ThumbPosition) && CBRADelegatesHolder.IsTouchingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });
        }
    }
}