using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.VRClicker;

/// <summary>
/// Event raised when an object was clicked using the VR Clicker system
/// </summary>
public class OnVRClickerStartClicking : EventCallbacks.Event<OnVRClickerStartClicking>
{
    /// <summary>
    /// The Origin of the ray that just clicked something
    /// </summary>
    public readonly ERayOrigin RaycastOrigin;

    /// <summary>
    /// The GameObject that was just clicked by the user (must have a collider)
    /// </summary>
    public readonly GameObject ClickedObject;

    /// <summary>
    /// Event raised when an object was clicked using the VR Clicker system
    /// </summary>
    /// <param name="rayOrigin">The Origin of the ray that just clicked something</param>
    /// <param name="objectClicked">The GameObject that was just clicked by the user (must have a collider)</param>
    public OnVRClickerStartClicking(ERayOrigin rayOrigin, GameObject objectClicked) : base("Event raised when an object was clicked using the VR Clicker system.")
    {
        RaycastOrigin = rayOrigin;
        ClickedObject = objectClicked;

        switch (rayOrigin)
        {
            case ERayOrigin.RIGHT_HAND:
                VRClickerVariablesContainer.CurrentClickedObjectRight = objectClicked;
                VRClickerVariablesContainer.IsClickingRight = true;
                break;
            case ERayOrigin.LEFT_HAND:
                VRClickerVariablesContainer.CurrentClickedObjectLeft = objectClicked;
                VRClickerVariablesContainer.IsClickingLeft = true;
                break;
            case ERayOrigin.CAMERA:
                VRClickerVariablesContainer.CurrentClickedObjectGaze = objectClicked;
                VRClickerVariablesContainer.IsClickingGaze = true;
                break;
        }

        FireEvent(this);
    }
}