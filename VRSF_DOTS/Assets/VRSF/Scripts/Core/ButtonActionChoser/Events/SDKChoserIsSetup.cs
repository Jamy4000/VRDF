namespace VRSF.Core.Events
{
    /// <summary>
    /// Used in the BAC Setup, raised when the different SDKChoser are setup
    /// </summary>
    public class SDKChoserIsSetup : EventCallbacks.Event<SDKChoserIsSetup>
    {
        public SDKChoserIsSetup() : base("Used in the BAC Setup, raised when the different SDKChoser are setup")
        {
            FireEvent(this);
        }
    }
}