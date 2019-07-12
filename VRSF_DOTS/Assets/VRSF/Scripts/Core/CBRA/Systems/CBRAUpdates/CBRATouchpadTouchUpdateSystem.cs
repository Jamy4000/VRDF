using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the touch on all controllers button EXCEPT the touchpad
    /// </summary>
    public class CBRATouchpadTouchUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref TouchpadInputCapture touchpadInput, ref CBRAThumbPosition cbraThumbPos, ref BaseInputCapture baseInput) => 
            {
                if (baseInput.IsClicking && ThumbPositionChecker.CheckThumbPosition(cbraThumbPos.TouchThumbPosition, cbraThumbPos.IsTouchingThreshold, touchpadInput.ThumbPosition))
                    CBRADelegatesHolder.IsTouchingEvents[entity]?.Invoke();
            });
        }
    }
}