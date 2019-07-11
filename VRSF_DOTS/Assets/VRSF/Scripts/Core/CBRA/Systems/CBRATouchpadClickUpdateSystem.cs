using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the clicks on all controllers button EXCEPT the touchpad
    /// </summary>
    public class CBRATouchpadClickUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref TouchpadInputCapture touchpadInput, ref CBRAThumbPosition cbraThumbPos, ref CBRAInteractionType cbraInteraction, ref BaseInputCapture baseInput) =>
            {
                if ((cbraInteraction.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK && baseInput.IsClicking && ThumbPositionChecker.CheckThumbPosition(cbraThumbPos.TouchThumbPosition, cbraThumbPos.IsTouchingThreshold, touchpadInput.ThumbPosition))
                    CBRADelegatesHolder.ClickEvents[cbraInteraction][ActionType.IsInteracting].Invoke();
            });
        }
    }
}