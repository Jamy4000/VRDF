using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// If the user is pressing/touching the flying button, we handle the acceleration
    /// </summary>
    public class FlyAccelerationCalculationsSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new AccelerationCalculationJob
            {
                DeltaTime = Time.DeltaTime
            }.Schedule(this, inputDeps);
        }
        
        [RequireComponentTag(typeof(IsFlying))]
        private struct AccelerationCalculationJob : IJobForEach<FlySpeed, FlyDirection, FlyAcceleration, FlyDeceleration>
        {
            public float DeltaTime;

            public void Execute(ref FlySpeed speed, ref FlyDirection direction, ref FlyAcceleration acceleration, ref FlyDeceleration deceleration)
            {
                if (deceleration.SlowDownTimer > 0.0f)
                {
                    acceleration.TimeSinceStartFlying = deceleration.SlowDownTimer;
                    deceleration.SlowDownTimer = 0.0f;
                }

                if (acceleration.TimeSinceStartFlying >= 0 && acceleration.TimeSinceStartFlying < 1.0f)
                    acceleration.TimeSinceStartFlying = acceleration.AccelerationEffectFactor != 0.0f ? acceleration.TimeSinceStartFlying + (DeltaTime / acceleration.AccelerationEffectFactor) : 1.0f;


                speed.CurrentFlightVelocity = speed.GetSpeed() * acceleration.TimeSinceStartFlying;
            }

            /// <summary>
            /// Override of the Math.Clamp method for the current flight velocity calculations.
            /// </summary>
            /// <param name="val"></param>
            /// <param name="srcMin"></param>
            /// <param name="srcMax"></param>
            /// <param name="dstMin"></param>
            /// <param name="dstMax"></param>
            /// <returns></returns>
            private float MapRangeClamp(float val, float srcMin, float srcMax, float dstMin, float dstMax)
            {
                if (val <= srcMin) return dstMin;
                else if (val >= srcMax) return dstMax;

                float denominator = (srcMax - srcMin) * (dstMax - dstMin);

                denominator = denominator == 0.0f ? 0.000001f : denominator;

                return dstMin + (val - srcMin) / denominator;
            }
        }
    }
}