﻿namespace VRSF.Core.Controllers
{
    /// <summary>
    /// List the possible state for the Left and Right pointers and the Gaze
    /// </summary>
	public enum EPointerState : int
    {
        ON = 0,
        OFF = 1,
        DISAPPEARING = 2
    }
}