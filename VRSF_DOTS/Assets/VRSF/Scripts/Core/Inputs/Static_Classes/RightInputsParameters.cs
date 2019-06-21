using Unity.Mathematics;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Used to handle the Right Inputs
    /// </summary>
    public static class RightInputsParameters
    {
        public static bool TriggerClick;
        public static bool TriggerTouch;
        public static float TriggerSqueezeValue;

        public static bool TouchpadClick;
        public static bool TouchpadTouch;
        public static float2 ThumbPosition;

        public static bool GripClick;
        public static bool GripTouch;
        public static float GripSqueezeValue;

        public static bool MenuClick;

        public static bool A_Click;
        public static bool A_Touch;

        public static bool B_Click;
        public static bool B_Touch;

        public static bool ThumbrestTouch;

        public static bool BackButtonClick;
    }
}