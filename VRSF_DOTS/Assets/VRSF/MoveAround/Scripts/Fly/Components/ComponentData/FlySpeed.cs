namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Contains all references to the speed variables.
    /// </summary>
    public struct FlySpeed : Unity.Entities.IComponentData
    {
        public float FlyingSpeedFactor;

        public float CurrentFlightVelocity;


        /// <summary>
        /// Get basic vertical speed (0.05) and multiply it by the flying speed factor
        /// </summary>
        /// <returns>The new vertical axis speed</returns>
        public float GetSpeed()
        {
            return 0.05f * FlyingSpeedFactor;
        }
    }
}