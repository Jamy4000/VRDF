using System;
using VRSF.Core.Inputs;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// 
    /// </summary>
	public static class InteractionChecker
    {
        public static bool IsInteractingSimple(BaseInputCapture bic, ControllersInteractionType cit)
        {
            return (cit.HasClickInteraction && bic.IsClicking) || (cit.HasTouchInteraction && bic.IsTouching);
        }

        public static bool IsNotInteracting(BaseInputCapture bic, ControllersInteractionType cit)
        {
            return (cit.HasClickInteraction && !bic.IsClicking) || (cit.HasTouchInteraction && !bic.IsTouching);
        }

        public static bool IsInteractingTouchpad(BaseInputCapture bic, ControllersInteractionType cit, InteractionThumbPosition itp, TouchpadInputCapture tic, bool checkingYAxis = false, bool checkingXAxis = true)
        {
            if (cit.HasClickInteraction && bic.IsClicking)
                return (checkingYAxis && Math.Abs(tic.ThumbPosition.y) > itp.IsClickingThreshold) || (checkingXAxis && Math.Abs(tic.ThumbPosition.x) > itp.IsClickingThreshold);
            
            if (cit.HasTouchInteraction && bic.IsTouching)
                return (checkingYAxis && Math.Abs(tic.ThumbPosition.y) > itp.IsTouchingThreshold) || (checkingXAxis && Math.Abs(tic.ThumbPosition.x) > itp.IsTouchingThreshold);
            
            return false;
        }

        public static bool IsNotInteractingTouchpad(BaseInputCapture bic, ControllersInteractionType cit, InteractionThumbPosition itp, TouchpadInputCapture tic, bool checkingYAxis = false, bool checkingXAxis = true)
        {
            if (cit.HasClickInteraction)
                return !bic.IsClicking || (checkingYAxis && Math.Abs(tic.ThumbPosition.y) < itp.IsClickingThreshold) || (checkingXAxis && Math.Abs(tic.ThumbPosition.x) < itp.IsClickingThreshold);
            
            if (cit.HasTouchInteraction)
                return !bic.IsTouching || (checkingYAxis && Math.Abs(tic.ThumbPosition.y) < itp.IsTouchingThreshold) || (checkingXAxis && Math.Abs(tic.ThumbPosition.x) < itp.IsTouchingThreshold);
            
            return false;
        }
    }
}