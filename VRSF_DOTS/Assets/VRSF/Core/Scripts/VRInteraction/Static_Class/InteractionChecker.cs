using VRSF.Core.Inputs;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// 
    /// </summary>
	public static class InteractionChecker
    {
        public static bool IsInteracting(BaseInputCapture bic, ControllersInteractionType cit)
        {
            return (cit.HasClickInteraction && bic.IsClicking) || (cit.HasTouchInteraction && bic.IsTouching);
        }

        public static bool IsNotInteracting(BaseInputCapture bic, ControllersInteractionType cit)
        {
            return (cit.HasClickInteraction && !bic.IsClicking) || (cit.HasTouchInteraction && !bic.IsTouching);
        }
    }
}