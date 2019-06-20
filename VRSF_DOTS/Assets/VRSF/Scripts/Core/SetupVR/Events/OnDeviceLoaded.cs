namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Event raised when the VR Device has been loaded
    /// </summary>
    public class OnDeviceLoaded : EventCallbacks.Event<OnDeviceLoaded>
    {
        public OnDeviceLoaded() : base("Event raised when the VR Device has been loaded")
        {
            FireEvent(this);
        }
    }
}