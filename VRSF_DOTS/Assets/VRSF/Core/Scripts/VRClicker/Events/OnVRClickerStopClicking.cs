using UnityEngine;
using VRSF.Core.Raycast;

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
    /// <param name="objectClicked">The GameObject that was unclicked by the user (must have a collider))</param>
    public OnVRClickerStopClicking(ERayOrigin rayOrigin, GameObject objectClicked) : base("Event raised when an object was being clicked and is not anymore, using the VR Clicker system.")
    {
        RaycastOrigin = rayOrigin;
        UnclickedObject = objectClicked;

        switch (rayOrigin)
        {
            case ERayOrigin.RIGHT_HAND:
                VRSF.Core.VRInteractions.InteractionVariableContainer.CurrentClickedObjectRight = null;
                VRSF.Core.VRInteractions.InteractionVariableContainer.IsClickingSomethingRight = false;
                break;
            case ERayOrigin.LEFT_HAND:
                VRSF.Core.VRInteractions.InteractionVariableContainer.CurrentClickedObjectLeft = null;
                VRSF.Core.VRInteractions.InteractionVariableContainer.IsClickingSomethingLeft = false;
                break;
            case ERayOrigin.CAMERA:
                VRSF.Core.VRInteractions.InteractionVariableContainer.CurrentClickedObjectGaze = null;
                VRSF.Core.VRInteractions.InteractionVariableContainer.IsClickingSomethingGaze = false;
                break;
        }

        FireEvent(this);
    }
}