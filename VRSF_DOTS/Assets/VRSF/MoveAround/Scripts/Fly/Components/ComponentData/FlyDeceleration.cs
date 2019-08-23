namespace VRSF.MoveAround.Fly
{
    public struct FlyDeceleration : Unity.Entities.IComponentData
    {
        /// <summary>
        /// The factor for the deceleration effect. Set to 0 to remove this effect.
        /// </summary>
        public float DecelerationEffectFactor;

        /// <summary>
        /// Timer since the user start slowing down
        /// </summary>
        public float SlowDownTimer;
    }
}