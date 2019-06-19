namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Event raised when the Crossplatform setup is finished
    /// </summary>
    public class OnCrossplatformComponentIsSetup : EventCallbacks.Event<OnCrossplatformComponentIsSetup>
    {
        public OnCrossplatformComponentIsSetup() : base("Event raised when the Crossplatform setup is finished.")
        {
            FireEvent(this);
        }
    }
}