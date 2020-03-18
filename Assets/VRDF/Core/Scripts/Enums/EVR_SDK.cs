﻿namespace VRDF.Core.SetupVR
{
    /// <summary>
    /// Group all type of SDK supported by the VRDF
    /// </summary>
    public enum EVR_SDK
    {
        NONE            = 0, 
        OCULUS          = 1 << 0,
        OPEN_VR         = 1 << 1
    }
}