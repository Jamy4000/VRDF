using VRSF.Core.Controllers;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// The base event to raise when the user stop touching a controllers button
    /// </summary>
    public class ButtonUntouchEvent : EventCallbacks.Event<ButtonUntouchEvent>
    {
        /// <summary>
        /// The button that was untouched
        /// </summary>
        public readonly EControllersButton ButtonInteracting;

        /// <summary>
        /// The hand on which the button is (LEFT or RIGHT)
        /// </summary>
        public readonly EHand HandInteracting;

        public ButtonUntouchEvent(EHand handInteracting, EControllersButton buttonInteracting) : base("The base event to raise when the user stop touching a controllers button.")
        {
            ButtonInteracting = buttonInteracting;
            HandInteracting = handInteracting;

            FireEvent(this);
        }
    }
}