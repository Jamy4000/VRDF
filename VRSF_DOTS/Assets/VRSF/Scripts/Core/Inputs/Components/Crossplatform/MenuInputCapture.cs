using Unity.Entities;
using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the Menu button of a controller
    /// </summary>
    public struct MenuInputCapture : IComponentData
    {
        /// <summary>
        /// The hand on which this menu button is placed
        /// </summary>
        public EHand Hand;

        public MenuInputCapture(EHand hand)
        {
            Hand = hand;
        }
    }
}