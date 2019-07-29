using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// If the user is pressing/touching the flying button, we handle the acceleration
    /// </summary>
    public class FlyAccelerationCalculationsSystem : JobComponentSystem
    {
        private Transform _cameraRigTrans;

        #region ComponentSystem_Methods
        protected override void OnCreate()
        {
            base.OnCreate();
            OnSetupVRReady.Listeners += Init;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new AccelerationCalculationJob
            {
                DeltaTime = Time.deltaTime,
                CameraRigYScale = _cameraRigTrans.lossyScale.y
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.Listeners -= Init;
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        [RequireComponentTag(typeof(IsFlying))]
        private struct AccelerationCalculationJob : IJobForEach<FlySpeed, FlyDirection, FlyAcceleration, FlyDeceleration, FlyBoundaries>
        {
            public float DeltaTime;
            public float CameraRigYScale;

            public void Execute(ref FlySpeed speed, ref FlyDirection direction, ref FlyAcceleration acceleration, ref FlyDeceleration deceleration, ref FlyBoundaries boundaries)
            {
                if (deceleration.SlowDownTimer > 0.0f)
                {
                    acceleration.TimeSinceStartFlying = deceleration.SlowDownTimer;
                    deceleration.SlowDownTimer = 0.0f;
                }

                if (acceleration.TimeSinceStartFlying >= 0 && acceleration.TimeSinceStartFlying < 1.0f)
                    acceleration.TimeSinceStartFlying = acceleration.AccelerationEffectFactor == 0.0f ? acceleration.TimeSinceStartFlying + (DeltaTime / acceleration.AccelerationEffectFactor) : 1.0f;


                speed.CurrentFlightVelocity = speed.GetSpeed() * acceleration.TimeSinceStartFlying;

                // We change the speed depending on the Scale of the User
                speed.CurrentFlightVelocity /= MapRangeClamp(CameraRigYScale, Mathf.Abs(boundaries.MinAvatarPosition.y), Mathf.Abs(boundaries.MaxAvatarPosition.y), 1.0f, boundaries.MaxAvatarPosition.y / 100);
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

        private void Init(OnSetupVRReady info)
        {
            _cameraRigTrans = VRSF_Components.CameraRig.transform;
        }
        #endregion PRIVATE_METHODS
    }
}