using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// The base event to raise when the user stop clicking on a controllers button
    /// </summary>
    public class ButtonUnclickEvent : EventCallbacks.Event<ButtonUnclickEvent>
    {
        /// <summary>
        /// The button that was unclicked
        /// </summary>
        public readonly EControllersButton ButtonInteracting;

        /// <summary>
        /// The hand on which the button is (LEFT or RIGHT)
        /// </summary>
        public readonly EHand HandInteracting;

        public ButtonUnclickEvent(EHand handInteracting, EControllersButton buttonInteracting) : base("The base event to raise when the user stop clicking on a controllers button.")
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}