using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Handle the clicks on the touchpads ONLY
    /// </summary>
    public class CBRATouchpadClickUpdateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            // Checking only the TouchpadInputCapture components as the others have their own dedicated system
            Entities.ForEach((Entity entity, ref TouchpadInputCapture touchpadInput, ref InteractionThumbPosition cbraThumbPos, ref BaseInputCapture baseInput) =>
            {
                if (baseInput.IsClicking && ThumbPositionChecker.CheckThumbPosition(cbraThumbPos.TouchThumbPosition, cbraThumbPos.IsTouchingThreshold, touchpadInput.ThumbPosition) && CBRADelegatesHolder.IsClickingEvents.TryGetValue(entity, out System.Action action))
                    action.Invoke();
            });
        }
    }
}