using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Clamp the current speed of the flying mode if we use boundaries
    /// </summary>
    [UpdateAfter(typeof(FlyDirectionCalculationSystem))]
    public class FlyBoundariesClamper : JobComponentSystem
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
            if (_cameraRigTrans == null)
                return inputDeps;

            return new BoundariesCalculationsJob
            {
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
        private struct BoundariesCalculationsJob : IJobForEach<FlySpeed, FlyBoundaries, FlyDirection>
        {
            public float CameraRigYScale;

            public void Execute(ref FlySpeed speed, ref FlyBoundaries boundaries, ref FlyDirection direction)
            {
                // We change the speed depending on the Scale of the User
                speed.CurrentFlightVelocity /= MapRangeClamp(CameraRigYScale, Mathf.Abs(boundaries.MinAvatarPosition.y), Mathf.Abs(boundaries.MaxAvatarPosition.y), 1.0f, boundaries.MaxAvatarPosition.y / 100);
                
                // If we use boundaries for the flying mode
                if (boundaries.UseBoundaries)
                {
                    // Clamp new values between min pos and max pos
                    var newPos = new float3
                    {
                        x = Mathf.Clamp(direction.CurrentDirection.x, boundaries.MinAvatarPosition.x, boundaries.MaxAvatarPosition.x),
                        y = Mathf.Clamp(direction.CurrentDirection.y, boundaries.MinAvatarPosition.y, boundaries.MaxAvatarPosition.y),
                        z = Mathf.Clamp(direction.CurrentDirection.z, boundaries.MinAvatarPosition.z, boundaries.MaxAvatarPosition.z)
                    };

                    direction.CurrentDirection = newPos;
                }
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