using System;
using UnityEngine;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    public static class CBRAInputTypeGetter
    {
        public static Type GetTypeFor(EControllersButton buttonInteracting)
        {
            switch (buttonInteracting)
            {
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

                case EControllersButton.BACK_BUTTON:
                    return typeof(GoAndGearVRInputCapture);

                case EControllersButton.TRIGGER:
                    return typeof(TriggerInputCapture);
                case EControllersButton.GRIP:
                    return typeof(GripInputCapture);
                case EControllersButton.MENU:
                    return typeof(MenuInputCapture);
                case EControllersButton.TOUCHPAD:
                    return typeof(TouchpadInputCapture);

                default:
                    Debug.LogErrorFormat("<b>[VRSF] :</b> Please Specify valid buttons to use for your ControllersButtonResponseAssigners.");
                    return null;
            }
        }
    }
}
