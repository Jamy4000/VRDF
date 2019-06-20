using Unity.Mathematics;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Used to handle the Left Inputs
    /// </summary>
    public static class LeftInputsParameters
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
    }
}