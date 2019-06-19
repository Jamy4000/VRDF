using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.Events
{
    /// <summary>
    /// Event raised when an object is clicked with the Trigger
    /// </summary>
    public class ObjectWasClickedEvent : EventCallbacks.Event<ObjectWasClickedEvent>
    {
        public readonly Transform ObjectClicked;
        public readonly ERayOrigin RayOrigin;

        public ObjectWasClickedEvent(ERayOrigin rayOrigin, Transform objectClicked) : base("Event raised when an object is clicked with the Trigger.")
        {
            RayOrigin = rayOrigin;
            ObjectClicked = objectClicked;

            FireEvent(this);
        }
    }
}