using UnityEngine;
using UnityEngine.EventSystems;
using VRSF.Core.Raycast;

namespace VRSF.Core.Events
{
    /// <summary>
    /// Event raised when an object is hovered by the laser
    /// </summary>
    public class ObjectWasHoveredEvent : EventCallbacks.Event<ObjectWasHoveredEvent>
    {
        public readonly Transform ObjectHovered;
        public readonly ERayOrigin RaycastOrigin;

        public ObjectWasHoveredEvent(ERayOrigin raycastOrigin, Transform objectHovered) : base("Event raised when an object is hovered by the laser.")
        {
            // We set the object that was Hovered as the selected gameObject
            if (objectHovered != null && objectHovered.GetComponent<UnityEngine.UI.Selectable>() != null)
                EventSystem.current.SetSelectedGameObject(objectHovered.gameObject);

            RaycastOrigin = raycastOrigin;
            ObjectHovered = objectHovered;

            FireEvent(this);
        }
    }
}