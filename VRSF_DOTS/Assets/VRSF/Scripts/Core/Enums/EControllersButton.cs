namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Contain all possible click type for Oculus and Vive controllers
    /// If you're using the two SDKs, make sure the button are corresponding.
    /// </summary>
    public enum EControllersButton
    {
        NONE = 0,
        TRIGGER = 1 << 0,
        GRIP = 1 << 1,
        TOUCHPAD = 1 << 2,
        MENU = 1 << 3,
        
        // OCULUS RIFT PARTICULARITIES
        A_BUTTON = 1 << 4,
        B_BUTTON = 1 << 5,
        X_BUTTON = 1 << 6,
        Y_BUTTON = 1 << 7,
        THUMBREST = 1 << 8,

        // OCULUS GO AND GEAR VR PARTICULARITIES
        BACK_BUTTON = 1 << 9
    }
}