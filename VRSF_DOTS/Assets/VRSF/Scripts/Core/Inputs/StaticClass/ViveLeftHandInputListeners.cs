using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    public static class ViveLeftHandInputListeners
    {
        #region TRIGGER
        public static void OnLeftTriggerDown()
        {
            LeftInputsParameters.TriggerClick = true;
            new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
        }

        public static void OnLeftTriggerUp()
        {
            LeftInputsParameters.TriggerClick = false;
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
        }
        #endregion TRIGGER

        #region GRIP
        public static void OnLeftGripDown()
        {
            LeftInputsParameters.GripClick = true;
            new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
        }

        public static void OnLeftGripUp()
        {
            LeftInputsParameters.GripClick = false;
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
        }
        #endregion GRIP

        #region TOUCHPAD
        public static void OnLeftTouchpadDown()
        {
            LeftInputsParameters.TouchpadClick = true;
            LeftInputsParameters.TouchpadTouch = false;
            new ButtonClickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadUp()
        {
            LeftInputsParameters.TouchpadClick = false;
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadTouch()
        {
            LeftInputsParameters.TouchpadTouch = true;
            new ButtonTouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadUntouch()
        {
            LeftInputsParameters.TouchpadTouch = false;
            new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadAxisChanged(Vector2 newAxis)
        {
            LeftInputsParameters.ThumbPosition = newAxis;
        }
        #endregion TOUCHPAD

        #region MENU
        public static void OnLeftMenuDown()
        {
            LeftInputsParameters.GripClick = true;
            new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
        }

        public static void OnLeftMenuUp()
        {
            LeftInputsParameters.GripClick = false;
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
        }
        #endregion MENU
    }
}
