namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Event raised when a VR Raycaster is done with its setup
    /// Mostly use in the UI System to check if a VRRaycaster is present in the scene
    /// </summary>
    public class OnVRRaycasterIsSetup : EventCallbacks.Event<OnVRRaycasterIsSetup>
    {
        /// <summary>
        /// Event raised when a VR Raycaster is done with its setup
        /// Mostly use in the UI System to check if a VRRaycaster is present in the scene
        /// </summary>
        public OnVRRaycasterIsSetup() : base("Event raised when a VR Raycaster is done with its setup")
        {
            FireEvent(this);
        }
    }
}