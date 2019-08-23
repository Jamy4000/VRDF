namespace VRSF.MoveAround.Fly
{
    public struct FlyAcceleration : Unity.Entities.IComponentData
    {
        /// <summary>
        /// The factor for the acceleration effect. Set to 0 to remove this effect.
        /// </summary>
        public float AccelerationEffectFactor;

        /// <summary>
        /// The current speed at which the user is going
        /// </summary>
        public float CurrentFlightVelocity;

        public float TimeSinceStartFlying;
    }
}