using UnityEngine;
using UnityEngine.EventSystems;
using VRSF.Core.Raycast;
using VRSF.Core.VRInteractions;

/// <summary>
/// Event raised when the user start to hover an object with a VR Raycaster
/// </summary>
public class OnStartHoveringObject : EventCallbacks.Event<OnStartHoveringObject>
{
    /// <summary>
    /// The Origin of the ray that just hovered something
    /// </summary>
    public readonly ERayOrigin RaycastOrigin;

    /// <summary>
    /// The GameObject that was just hovered by the user (must have a collider)
    /// </summary>
    public readonly GameObject HoveredObject;

    /// <summary>
    /// Event raised when the user start to hover an object with a VR Raycaster
    /// </summary>
    /// <param name="raycastOrigin">The Origin of the ray that just hovered something</param>
    /// <param name="objectHovered">The GameObject that was just hovered by the user (must have a collider)</param>
    public OnStartHoveringObject(ERayOrigin raycastOrigin, GameObject objectHovered, Vector3 hitPoint) : base("Event raised when the user start to hover an object with a VR Raycaster")
    {
        // We set the object that was Hovered as the selected gameObject
        if (objectHovered != null)
        {
            EventSystem.current.SetSelectedGameObject(objectHovered.gameObject);

            var selectableObject = objectHovered.GetComponent<UnityEngine.UI.Selectable>();
            if (selectableObject != null)
                selectableObject.OnSelect(null);
        }

        RaycastOrigin = raycastOrigin;
        HoveredObject = objectHovered;

        FireEvent(this);
    }
}