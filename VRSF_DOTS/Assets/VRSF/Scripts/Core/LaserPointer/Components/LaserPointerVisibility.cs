using Unity.Entities;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Contains all the variable for the ControllerPointer Systems
    /// </summary>
    [RequireComponentTag(typeof(Raycast.VRRaycastOrigin))]
    public struct LaserPointerVisibility : IComponentData
    {
        /// <summary>
        /// How fast the pointer is disappearing when not hitting something
        /// </summary>
        public float DisappearanceSpeed;
    }
} 