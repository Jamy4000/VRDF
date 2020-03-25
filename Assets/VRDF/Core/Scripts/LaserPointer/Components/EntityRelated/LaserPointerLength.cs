using Unity.Entities;

namespace VRDF.Core.LaserPointer
{
    /// <summary>
    /// Contains the mandatory variables to calculate the length of the laser, as well as its end position
    /// </summary>
    public struct LaserPointerLength : IComponentData
    {
        /// <summary>
        /// Should the laser be pointing at the center of the 3D Object it is currently hitting ?
        /// </summary>
        public bool ShouldPointTo3DObjectsCenter;

        /// <summary>
        /// Should the laser be pointing at the center of the UI Object it is currently hitting ?
        /// </summary>
        public bool ShouldPointToUICenter;

        /// <summary>
        /// The base length of the laser, set using the VRRaycastAuthoring.MaxRaycastDistance value
        /// </summary>
        public float BaseLength;
    }
}