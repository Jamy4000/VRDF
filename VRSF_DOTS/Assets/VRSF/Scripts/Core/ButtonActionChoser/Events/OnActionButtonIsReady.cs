namespace VRSF.Core.Events
{
    /// <summary>
    /// Used in the BAC Setup, raised when the actions buttons are setup correctly
    /// </summary>
    public class OnActionButtonIsReady : EventCallbacks.Event<OnActionButtonIsReady>
    {
        public OnActionButtonIsReady() : base("Used in the BAC Setup, raised when the actions buttons are setup correctly")
        {
            FireEvent(this);
        }
    }
}