using Unity.Entities;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Used to calculate the disappearance speed of the laser
    /// </summary>
    public struct LaserPointerVisibility : IComponentData
    {
        /// <summary>
        /// How fast the pointer is disappearing when not hitting something
        /// </summary>
        public float DisappearanceSpeed;

        /// <summary>
        /// The base width to reset the laser when hitting something
        /// </summary>
        public float BaseWidth;

        /// <summary>
        /// The current width of the laser
        /// </summary>
        public float CurrentWidth;
    }
} 