using System;
using UnityEngine;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Helper class to get a type of IComponentData based on a controller button interacting, or EControllersButton
    /// </summary>
    public static class CBRAInputTypeGetter
    {
        /// <summary>
        /// Get a type of IComponentData based on a controller button interacting, or EControllersButton
        /// </summary>
        /// <param name="buttonInteracting">The requested button on the controller</param>
        /// <returns>The corresponding Type (or IComponentData) corresponding to the requested button</returns>
        public static Type GetTypeFor(EControllersButton buttonInteracting)
        {
            switch (buttonInteracting)
            {
                // basic buttons, on all controller
                case EControllersButton.TRIGGER:
                    return typeof(TriggerInputCapture);
                case EControllersButton.GRIP:
                    return typeof(GripInputCapture);
                case EControllersButton.MENU:
                    return typeof(MenuInputCapture);
                case EControllersButton.TOUCHPAD:
                    return typeof(TouchpadInputCapture);

                // Oculus Specific buttons
                case EControllersButton.A_BUTTON:
                    return typeof(AButtonInputCapture);
                case EControllersButton.B_BUTTON:
                    return typeof(BButtonInputCapture);
                case EControllersButton.X_BUTTON:
                    return typeof(XButtonInputCapture);
                case EControllersButton.Y_BUTTON:
                    return typeof(YButtonInputCapture);
                case EControllersButton.THUMBREST:
                    return typeof(ThumbrestInputCapture);

                // Go and GearVR specific buttons
                case EControllersButton.BACK_BUTTON:
                    return typeof(GoAndGearVRInputCapture);

                default:
                    Debug.LogErrorFormat("<Color=red><b>[VRSF] :</b> Please Specify valid buttons to use for your ControllersButtonResponseAssigners.</Color>");
                    return null;
            }
        }
    }
}
