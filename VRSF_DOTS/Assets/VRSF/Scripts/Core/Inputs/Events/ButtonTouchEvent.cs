using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// The base event to raise when the user start touching a controllers button
    /// </summary>
    public class ButtonTouchEvent : EventCallbacks.Event<ButtonTouchEvent>
    {
        /// <summary>
        /// The button that was touched
        /// </summary>
        public readonly EControllersButton ButtonInteracting;

        /// <summary>
        /// The hand on which the button is (LEFT or RIGHT)
        /// </summary>
        public readonly EHand HandInteracting;

        public ButtonTouchEvent(EHand handInteracting, EControllersButton buttonInteracting) : base("The base event to raise when the user start touching a controllers button.")
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}