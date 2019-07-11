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
            Entities.ForEach((ref TouchpadInputCapture touchpadInput, ref CBRAThumbPosition cbraThumbPos, ref CBRAInteractionType cbraInteraction, ref BaseInputCapture baseInput) => 
            {
                if ((cbraInteraction.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH && baseInput.IsClicking && ThumbPositionChecker.CheckThumbPosition(cbraThumbPos.TouchThumbPosition, cbraThumbPos.IsTouchingThreshold, touchpadInput.ThumbPosition))
                    CBRADelegatesHolder.TouchEvents[cbraInteraction][ActionType.IsInteracting].Invoke();
            });
        }
    }
}