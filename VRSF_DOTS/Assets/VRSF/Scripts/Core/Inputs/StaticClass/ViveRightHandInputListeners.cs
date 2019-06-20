using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    public static class ViveRightHandInputListeners
    {
        #region TRIGGER
        public static void OnRightTriggerDown()
        {
            RightInputsParameters.TriggerClick = true;
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
        }

        public static void OnRightTriggerUp()
        {
            RightInputsParameters.TriggerClick = false;
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
        }
        #endregion TRIGGER

        #region GRIP
        public static void OnRightGripDown()
        {
            RightInputsParameters.GripClick = true;
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }

        public static void OnRightGripUp()
        {
            RightInputsParameters.GripClick = false;
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }
        #endregion GRIP

        #region TOUCHPAD
        public static void OnRightTouchpadDown()
        {
            RightInputsParameters.TouchpadClick = true;
            RightInputsParameters.TouchpadTouch = false;
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadUp()
        {
            RightInputsParameters.TouchpadClick = false;
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadTouch()
        {
            RightInputsParameters.TouchpadTouch = true;
            new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadUntouch()
        {
            RightInputsParameters.TouchpadTouch = false;
            new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadAxisChanged(Vector2 newAxis)
        {
            RightInputsParameters.ThumbPosition = newAxis;
        }
        #endregion TOUCHPAD

        #region MENU
        public static void OnRightMenuDown()
        {
            RightInputsParameters.GripClick = true;
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }

        public static void OnRightMenuUp()
        {
            RightInputsParameters.GripClick = false;
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }
        #endregion MENU
    }
}
