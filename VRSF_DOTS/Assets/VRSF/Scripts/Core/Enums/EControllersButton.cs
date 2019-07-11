namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Contain all possible click type for Oculus and Vive controllers
    /// If you're using the two SDKs, make sure the button are corresponding.
    /// </summary>
    public enum EControllersButton
    {
        NONE,
        TRIGGER,
        GRIP,
        TOUCHPAD,
        MENU,
        
        // OCULUS RIFT PARTICULARITIES
        A_BUTTON,
        B_BUTTON,
        X_BUTTON,
        Y_BUTTON,
        THUMBREST,

        // OCULUS GO AND GEAR VR PARTICULARITIES
        BACK_BUTTON
    }
}