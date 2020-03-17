using Unity.Entities;

namespace VRSF.Core.Simulator
{
    /// <summary>
    /// Components to calculate the horizontal movements of the simulator using the arrow keys or WASD/ZQSD
    /// </summary>
    public struct SimulatorMovements : IComponentData
    {
        /// <summary>
        /// Base speed for moving on the horizontal axis
        /// </summary>
        public float WalkSpeed;

        /// <summary>
        /// The boost effect to apply when the user press one of the shift key
        /// </summary>
        public float ShiftBoost;

        /// <summary>
        /// Should the player always stay on the floor ? Use raycasting to check for collider under the Camera.
        /// </summary>
        public bool IsGrounded;
    }
}