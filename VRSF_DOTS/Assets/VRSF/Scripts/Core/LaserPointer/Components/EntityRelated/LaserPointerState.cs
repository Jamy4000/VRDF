using Unity.Entities;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    [RequireComponentTag(typeof(VRRaycastOrigin))]
    public struct LaserPointerState : IComponentData
    {
        /// <summary>
        /// The current state of the Pointer.
        /// </summary>
        public EPointerState State;

        public bool StateJustChangedToOn;
    }
}