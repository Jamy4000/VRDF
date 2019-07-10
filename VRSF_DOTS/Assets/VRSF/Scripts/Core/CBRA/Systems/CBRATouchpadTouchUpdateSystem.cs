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
            Entities.ForEach((ref TouchpadInputCapture touchpadInput, ref CBRAThumbPosition cbraThumbPos, ref CBRATouchEvents cbraTouchEvents, ref BaseInputCapture baseInput) => 
            {
                if (baseInput.IsClicking && ThumbPositionChecker.CheckThumbPosition(cbraThumbPos.TouchThumbPosition, cbraThumbPos.IsTouchingThreshold, touchpadInput.ThumbPosition))
                    cbraTouchEvents.OnButtonIsTouching.Invoke();
            });
        }
    }
}