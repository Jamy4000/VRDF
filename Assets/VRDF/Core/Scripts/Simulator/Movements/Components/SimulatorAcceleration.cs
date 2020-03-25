using Unity.Entities;

namespace VRDF.Core.Simulator
{
    /// <summary>
    /// Components to check the acceleration of the Simulator when moving around
    /// </summary>
    public struct SimulatorAcceleration : IComponentData
    {
        /// <summary>
        /// Maximum acceleration factor to apply to the base speed.
        /// </summary>
        public float MaxAccelerationFactor;

        /// <summary>
        /// How fast is the acceleration effect going. The higher the number is, the slower you gonna accelerate.
        /// </summary>
        public float AccelerationSpeed;

        /// <summary>
        /// TImer for the acceleration calculations
        /// </summary>
        public float AccelerationTimer;
    }
}