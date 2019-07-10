using Unity.Entities;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the Thumbrest of an Oculus Rift, Rift S and Quest controller
    /// </summary>
    public struct ThumbrestInputCapture : IComponentData
    {
        /// <summary>
        /// The hand on which this thumbrest is placed
        /// </summary>
        public EHand Hand;

        public ThumbrestInputCapture(EHand hand)
        {
            Hand = hand;
        }
    }
}