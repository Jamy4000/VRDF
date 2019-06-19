using UnityEngine;

namespace VRSF.Core.Inputs
{
    public static class ControllerInputToSO
    {
        public static string GetClickVariableFor(EControllersButton input)
        {
            switch (input)
            {
                case (EControllersButton.TRIGGER):
                    return "TriggerIsDown";

                case (EControllersButton.GRIP):
                    return "GripIsDown";

                case (EControllersButton.MENU):
                    return "MenuIsDown";

                case (EControllersButton.TOUCHPAD):
                    return "ThumbIsDown";

                case (EControllersButton.A_BUTTON):
                    return "AButtonIsDown";
                case (EControllersButton.B_BUTTON):
                    return "BButtonIsDown";
                case (EControllersButton.X_BUTTON):
                    return "XButtonIsDown";
                case (EControllersButton.Y_BUTTON):
                    return "YButtonIsDown";

                default:
                    Debug.LogError("The EControllersInput provided is null.");
                    return null;
            }
        }


        public static string GetTouchVariableFor(EControllersButton input)
        {
            switch (input)
            {
                case (EControllersButton.TOUCHPAD):
                    return "ThumbIsTouching";

                case (EControllersButton.TRIGGER):
                    return "TriggerIsTouching";

                case (EControllersButton.A_BUTTON):
                    return "AButtonIsTouching";
                case (EControllersButton.B_BUTTON):
                    return "BButtonIsTouching";
                case (EControllersButton.X_BUTTON):
                    return "XButtonIsTouching";
                case (EControllersButton.Y_BUTTON):
                    return "YButtonIsTouching";

                case (EControllersButton.THUMBREST):
                    return "ThumbrestIsTouching";

                default:
                    Debug.LogError("The EControllersInput provided is not correct.");
                    return null;
            }
        }
    }
}