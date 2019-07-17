using Unity.Entities;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the Trigger of a controller
    /// </summary>
    public struct TriggerInputCapture : IComponentData
    {
        /// <summary>
        /// The hand on which this trigger button is placed
        /// </summary>
        public EHand Hand;

        /// <summary>
        /// Until which point do we consider a squeeze on the trigger as touch or as click
        /// </summary>
        public float SqueezeClickThreshold;

        /// <summary>
        /// At which point the user is squeezing the trigger, between 0.0f and 1.0f
        /// </summary>
        public float TriggerSqueezeValue;
        
        public TriggerInputCapture(EHand hand, float squeezeClickThreshold = 0.95f)
        {
            Hand = hand;
            SqueezeClickThreshold = squeezeClickThreshold;
            TriggerSqueezeValue = 0.0f;
        }
    }
}