namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Event raised when the setting up of the VR Components are done
    /// </summary>
    public class OnSetupVRNeedToBeReloaded : EventCallbacks.Event<OnSetupVRNeedToBeReloaded>
    {
        public OnSetupVRNeedToBeReloaded() : base("Event raised when we need to reload SetupVR")
        {
            FireEvent(this);
        }
    }
}