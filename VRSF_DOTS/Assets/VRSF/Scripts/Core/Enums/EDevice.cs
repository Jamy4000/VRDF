namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// Group all type of Devices supported by the VRSF
    /// </summary>
    public enum EDevice
    {
        NONE            = 0,        //To handle errors
        SIMULATOR       = 1 << 0,
        HTC_VIVE        = 1 << 1,
        HTC_FOCUS       = 1 << 2,
        OCULUS_RIFT     = 1 << 3,
        OCULUS_RIFT_S   = 1 << 4,
        OCULUS_GO       = 1 << 5,
        OCULUS_QUEST    = 1 << 6,
        GEAR_VR         = 1 << 7,
        WMR             = 1 << 8,
        ALL             = ~0
    }
}