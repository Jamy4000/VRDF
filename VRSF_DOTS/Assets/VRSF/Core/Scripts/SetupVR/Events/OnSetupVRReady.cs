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

        /// <summary>
        /// Used to register an listener to this event. if SetupVR is already Ready, we call the method.
        /// </summary>
        /// <param name="listener">The listener to add to OnSetupVRReady Listeners, or to call if SetupVR is already ready</param>
        /// <param name="shouldStillRegister">Should we still register the listener if SetupVR is Ready</param>
        public static void RegisterSetupVRResponse(EventListener listener, bool shouldStillRegister = false)
        {
            if (VRSF_Components.SetupVRIsReady)
            {
                listener(null);
                if (shouldStillRegister)
                    Listeners += listener;
            }
            else
            {
                Listeners += listener;
            }
        }
    }
}