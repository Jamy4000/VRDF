using Unity.Entities;

namespace VRDF.Core.LaserPointer
{
    /// <summary>
    /// Handle the actual state of the laser pointer
    /// </summary>
    public struct LaserPointerState : IComponentData
    {
        /// <summary>
        /// The current state of the Pointer.
        /// </summary>
        public EPointerState State;

        /// <summary>
        /// Check if the State variable just switch to ON
        /// </summary>
        public bool StateJustChangedToOn;
    }
}