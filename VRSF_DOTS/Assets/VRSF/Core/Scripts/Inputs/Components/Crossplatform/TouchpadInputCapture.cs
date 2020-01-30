using Unity.Entities;
using Unity.Mathematics;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the Touchpad of a controller
    /// </summary>
    public struct TouchpadInputCapture : IComponentData
    {
        /// <summary>
        /// where is the user's finger on the touchpad ?
        /// </summary>
        public float2 ThumbPosition;

        public bool UseThumbPosForTouch;

        public TouchpadInputCapture(bool useThumbPosForTouch)
        {
            ThumbPosition = float2.zero;
            UseThumbPosForTouch = useThumbPosForTouch;
        }
    }
}