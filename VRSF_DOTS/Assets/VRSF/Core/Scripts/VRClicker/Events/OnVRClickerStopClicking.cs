using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.VRClicker;

/// <summary>
/// Event raised when an object was being clicked and is not anymore, using the VR Clicker system
/// </summary>
public class OnVRClickerStopClicking : EventCallbacks.Event<OnVRClickerStopClicking>
{
    /// <summary>
    /// The Origin of the ray that just clicked something
    /// </summary>
    public readonly ERayOrigin RaycastOrigin;

    /// <summary>
    /// The GameObject that was unclicked by the user (must have a collider)
    /// </summary>
    public readonly GameObject UnclickedObject;

    /// <summary>
    /// Event raised when an object was being clicked and is not anymore, using the VR Clicker system
    /// </summary>
    /// <param name="rayOrigin">The Origin of the ray that just clicked something</param>
    /// <param name="unclickedObject">The GameObject that was unclicked by the user (must have a collider))</param>
    public OnVRClickerStopClicking(ERayOrigin rayOrigin, GameObject unclickedObject) : base("Event raised when an object was being clicked and is not anymore, using the VR Clicker system.")
    {
        RaycastOrigin = rayOrigin;
        UnclickedObject = unclickedObject;

        VRClickerVariablesContainer.SetCurrentClickedVariables(rayOrigin, null, false);

        FireEvent(this);
    }
}