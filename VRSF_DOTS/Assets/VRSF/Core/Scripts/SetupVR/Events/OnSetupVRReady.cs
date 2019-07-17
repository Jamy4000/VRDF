namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Event raised when the setting up of the VR Components are done
    /// </summary>
    public class OnSetupVRReady : EventCallbacks.Event<OnSetupVRReady>
    {
        public OnSetupVRReady() : base("Event raised when the setting up of the VR Components are done")
        {
            FireEvent(this);
        }
    }
}