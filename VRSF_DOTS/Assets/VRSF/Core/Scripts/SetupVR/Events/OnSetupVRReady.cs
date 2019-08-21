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

        public static void RegisterSetupVRResponse(EventListener listener)
        {
            if (VRSF_Components.SetupVRIsReady)
                listener(null);
            else
                Listeners += listener;
        }
    }
}