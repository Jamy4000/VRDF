using Unity.Entities;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the Grip of a controller
    /// </summary>
    public struct GripInputCapture : IComponentData
    {
        /// <summary>
        /// The hand on which this grip button is placed
        /// </summary>
        public EHand Hand;

        /// <summary>
        /// Until which point do we consider a squeeze on the grip as touch or as click
        /// </summary>
        public float SqueezeClickThreshold;

        /// <summary>
        /// At which point the user is squeezing the grip, between 0.0f and 1.0f
        /// </summary>
        public float GripSqueezeValue;
        
        public GripInputCapture(EHand hand, float squeezeClickThreshold = 0.95f)
        {
            Hand = hand;
            SqueezeClickThreshold = squeezeClickThreshold;
            GripSqueezeValue = 0.0f;
        }
    }
}