using Unity.Entities;

namespace VRSF.Core.Simulator
{
    public struct SimulatorMovementRotation : IComponentData
    {
        /// <summary>
        /// Time it takes to interpolate camera rotation 99% of the way to the target.
        /// </summary>
        public float RotationLerpTime;

        /// <summary>
        /// Whether the Rotation of the camera in the Y axis should be inversed
        /// </summary>
        public bool ReversedYAxis;
    }
}
