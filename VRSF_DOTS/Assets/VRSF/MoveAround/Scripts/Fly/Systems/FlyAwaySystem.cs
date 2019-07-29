using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// System Allowing the user to fly with the thumbstick / touchpad. 
    /// </summary>
    public class FlyAwaySystem : JobComponentSystem
    {
        private Transform _cameraRigTransform;

        #region ComponentSystem_Methods
        protected override void OnCreate()
        {
            base.OnCreate();
            OnSetupVRReady.Listeners += Init;
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_cameraRigTransform == null)
                return inputDeps;

            NativeArray<float3> newPos = new NativeArray<float3>(1, Allocator.TempJob);
            NativeArray<bool> hasMoved = new NativeArray<bool>(1, Allocator.TempJob);

            var flyJob = new FlyAwayJob
            {
                NewPos = newPos,
                HasMoved = hasMoved,
                CameraRigTrans = _cameraRigTransform
            }.Schedule(this, inputDeps);

            flyJob.Complete();

            // Set avatar position
            if (hasMoved[0])
                VRSF_Components.CameraRig.transform.position = newPos[0];

            hasMoved.Dispose();
            newPos.Dispose();

            return inputDeps;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.Listeners -= Init;
        }
        #endregion ComponentSystem_Methods


        #region PRIVATE_METHODS
        [RequireComponentTag(typeof(IsFlying))]
        private struct FlyAwayJob : IJobForEach<FlySpeed, FlyDirection, VRRaycastOutputs, FlyBoundaries>
        {
            public NativeArray<float3> NewPos;
            public NativeArray<bool> HasMoved;

            public Transform CameraRigTrans;

            public void Execute(ref FlySpeed speed, ref FlyDirection direction, [ReadOnly] ref VRRaycastOutputs raycastOutputs, [ReadOnly] ref FlyBoundaries boundaries)
            {
                // We get the new position of the user according to where he's pointing/looking
                NewPos[0] = GetNewPosition(ref speed, ref direction, boundaries, raycastOutputs, CameraRigTrans);

                // If we use boundaries for the flying mode
                if (boundaries.UseBoundaries)
                {
                    // Clamp new values between min pos and max pos
                    var newPos = new float3
                    {
                        x = Mathf.Clamp(NewPos[0].x, boundaries.MinAvatarPosition.x, boundaries.MaxAvatarPosition.x),
                        y = Mathf.Clamp(NewPos[0].y, boundaries.MinAvatarPosition.y, boundaries.MaxAvatarPosition.y),
                        z = Mathf.Clamp(NewPos[0].z, boundaries.MinAvatarPosition.z, boundaries.MaxAvatarPosition.z)
                    };

                    NewPos[0] = newPos;
                }
            }
            
            /// <summary>
            /// Check if the user fly forward or backward
            /// </summary>
            /// <returns>The new position without the boundaries</returns>
            private float3 GetNewPosition(ref FlySpeed speed, ref FlyDirection direction, FlyBoundaries boundaries, VRRaycastOutputs raycastOutputs, Transform cameraRigTrans)
            {
                direction.NormalizedDir = Vector3.Normalize(raycastOutputs.RayVar.direction);
                direction.FinalDirection = direction.NormalizedDir * speed.CurrentFlightVelocity * direction.FlightDirection;

                return (float3)cameraRigTrans.position + direction.FinalDirection;
            }
        }
        
        private void Init(OnSetupVRReady info)
        {
            _cameraRigTransform = VRSF_Components.CameraRig.transform;
        }
        #endregion PRIVATE_METHODS
    }
}