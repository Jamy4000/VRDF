namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Base COmponent to check for clicks from the user using the VRClickerAuthoring
    /// </summary>
    public struct VRClicker : Unity.Entities.IComponentData
    {
        /// <summary>
        /// The button we want to check when clicking, default should be trigger
        /// </summary>
        public Inputs.EControllersButton ControllersButton;

        /// <summary>
        /// the hand used to check for click with this pointer
        /// </summary>
        public Controllers.EHand HandClicking;

        /// <summary>
        /// Whether the user is able to click on stuffs
        /// </summary>
        public bool CanClick;

        /// <summary>
        /// Whether we started to click.
        /// </summary>
        public bool IsClicking;

        /// <summary>
        /// Whether we already checked the StartClickingEventComp.
        /// </summary>
        public bool HasCheckedStartClickingEvent;

        /// <summary>
        /// Whether we already checked the StopClickingEventComp.
        /// </summary>
        public bool HasCheckedStopClickingEvent;
    }
}