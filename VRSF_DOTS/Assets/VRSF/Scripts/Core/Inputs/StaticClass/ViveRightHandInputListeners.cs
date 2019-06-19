using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    public static class ViveRightHandInputListeners
    {
        public static InputParameters RightInputParam;

        #region TRIGGER
        public static void OnRightTriggerDown()
        {
            RightInputParam.ClickBools.Get("TriggerIsDown").SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
        }

        public static void OnRightTriggerUp()
        {
            RightInputParam.ClickBools.Get("TriggerIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
        }
        #endregion TRIGGER

        #region GRIP
        public static void OnRightGripDown()
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }

        public static void OnRightGripUp()
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }
        #endregion GRIP

        #region TOUCHPAD
        public static void OnRightTouchpadDown()
        {
            RightInputParam.ClickBools.Get("ThumbIsDown").SetValue(true);
            RightInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadUp()
        {
            RightInputParam.ClickBools.Get("ThumbIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadTouch()
        {
            RightInputParam.TouchBools.Get("ThumbIsTouching").SetValue(true);
            new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadUntouch()
        {
            RightInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
        }

        public static void OnRightTouchpadAxisChanged(Vector2 newAxis)
        {
            RightInputParam.ThumbPosition.SetValue(newAxis);
        }
        #endregion TOUCHPAD

        #region MENU
        public static void OnRightMenuDown()
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }

        public static void OnRightMenuUp()
        {
            RightInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
        }
        #endregion MENU
    }
}
