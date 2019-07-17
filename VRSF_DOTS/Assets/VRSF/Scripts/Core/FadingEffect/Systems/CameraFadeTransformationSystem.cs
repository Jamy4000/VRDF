using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using VRSF.Core.SetupVR;
using Unity.Jobs;
using Unity.Mathematics;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeTransformationSystem : JobComponentSystem
    {
        private Transform _vrCamera;

        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += Init;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_vrCamera == null)
                return inputDeps;

            var pos = new NativeArray<float3>(1, Allocator.TempJob);
            pos[0] = _vrCamera.transform.position + _vrCamera.transform.forward * 0.3f;

            var rot = new NativeArray<quaternion>(1, Allocator.TempJob);
            rot[0] = Quaternion.LookRotation(_vrCamera.transform.up, _vrCamera.transform.forward);

            return new CameraFollowJob
            {
                CameraPos = pos,
                CameraRot = rot
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.Listeners -= Init;
        }


        [Unity.Burst.BurstCompile]
        public struct CameraFollowJob : IJobForEach<Translation, Rotation, CameraFadeParameters>
        {
            [DeallocateOnJobCompletion]
            public NativeArray<float3> CameraPos;
            [DeallocateOnJobCompletion]
            public NativeArray<quaternion> CameraRot;

            public void Execute(ref Translation camPosition, ref Rotation camRotation, [ReadOnly] ref CameraFadeParameters c2)
            {
                // Place the instantiated canvas in front of the camera
                camPosition.Value = CameraPos[0];

                // Rotate the instantiated canvas to face the camera
                camRotation.Value = CameraRot[0];
            }
        }

        private void Init(OnSetupVRReady info)
        {
            _vrCamera = VRSF_Components.VRCamera.transform;
        }
    }
}