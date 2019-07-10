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
        /// where is the user's finger on the touchpad ?
        /// </summary>
        public float2 ThumbPosition;

        public TouchpadInputCapture(EHand hand)
        {
            Hand = hand;
            ThumbPosition = float2.zero;
        }
    }
}