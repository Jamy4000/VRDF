using Unity.Entities;

namespace VRSF.Core.Simulator
{
    /// <summary>
    /// Components to calculate the rotation of the camera in SImualtor mode using right click
    /// </summary>
    public struct SimulatorRotation : IComponentData
    {
        /// <summary>
        /// Base speed for rotating the camera
        /// </summary>
        public float RotationSpeed;
    }
}