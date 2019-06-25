using Unity.Entities;
using Unity.Mathematics;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the Touchpad of a controller
    /// </summary>
    public struct TouchpadInputCapture : IComponentData
    {
        /// <summary>
        /// The hand on which this touchpad is placed
        /// </summary>
        public EHand Hand;

        /// <summary>
        /// Is the User clicking on the touchpad/thumbstick ?
        /// </summary>
        public bool TouchpadClick;

        /// <summary>
        /// Is the User touching on the touchpad/thumbstick ?
        /// </summary>
        public bool TouchpadTouch;

        /// <summary>
        /// where is the user's finger on the touchpad ?
        /// </summary>
        public float2 ThumbPosition;

        public TouchpadInputCapture(EHand hand)
        {
            Hand = hand;
            TouchpadClick = false;
            TouchpadTouch = false;
            ThumbPosition = float2.zero;
        }
    }
}