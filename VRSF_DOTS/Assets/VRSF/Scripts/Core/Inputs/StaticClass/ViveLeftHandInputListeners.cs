using UnityEngine;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    public static class ViveLeftHandInputListeners
    {
        public static InputParameters LeftInputParam;

        private static bool _triggerIsTouching;

        #region TRIGGER
        public static void OnLeftTriggerDown()
        {
            LeftInputParam.ClickBools.Get("TriggerIsDown").SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
        }

        public static void OnLeftTriggerUp()
        {
            LeftInputParam.ClickBools.Get("TriggerIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
        }
        #endregion TRIGGER

        #region GRIP
        public static void OnLeftGripDown()
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
        }

        public static void OnLeftGripUp()
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
        }
        #endregion GRIP

        #region TOUCHPAD
        public static void OnLeftTouchpadDown()
        {
            LeftInputParam.ClickBools.Get("ThumbIsDown").SetValue(true);
            LeftInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadUp()
        {
            LeftInputParam.ClickBools.Get("ThumbIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadTouch()
        {
            LeftInputParam.TouchBools.Get("ThumbIsTouching").SetValue(true);
            new ButtonTouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadUntouch()
        {
            LeftInputParam.TouchBools.Get("ThumbIsTouching").SetValue(false);
            new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
        }

        public static void OnLeftTouchpadAxisChanged(Vector2 newAxis)
        {
            LeftInputParam.ThumbPosition.SetValue(newAxis);
        }
        #endregion TOUCHPAD

        #region MENU
        public static void OnLeftMenuDown()
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(true);
            new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
        }

        public static void OnLeftMenuUp()
        {
            LeftInputParam.ClickBools.Get("GripIsDown").SetValue(false);
            new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
        }
        #endregion MENU
    }
}
