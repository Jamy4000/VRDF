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
            OnSetupVRReady.Listeners += InitSetupVR;
            StartFadingInEvent.Listeners += InitFadingIn;
            StartFadingOutEvent.Listeners += InitFadingOut;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (_vrCamera == null)
                return inputDeps;

            var pos = _vrCamera.transform.position + _vrCamera.transform.forward * 0.3f;
            var rot = Quaternion.LookRotation(_vrCamera.transform.up, _vrCamera.transform.forward);

            return new CameraFollowJob
            {
                CameraPos = pos,
                CameraRot = rot
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.Listeners -= InitSetupVR;
            StartFadingInEvent.Listeners -= InitFadingIn;
            StartFadingOutEvent.Listeners -= InitFadingOut;
        }


        [Unity.Burst.BurstCompile]
        public struct CameraFollowJob : IJobForEach<Translation, Rotation, CameraFadeParameters>
        {
            public float3 CameraPos;
            public quaternion CameraRot;

            public void Execute(ref Translation camPosition, ref Rotation camRotation, [ReadOnly] ref CameraFadeParameters c2)
            {
                // Place the instantiated canvas in front of the camera
                camPosition.Value = CameraPos;

                // Rotate the instantiated canvas to face the camera
                camRotation.Value = CameraRot;
            }
        }

        private void InitSetupVR(OnSetupVRReady _)
        {
            if (VRSF_Components.VRCamera != null)
                _vrCamera = VRSF_Components.VRCamera.transform;
        }

        private void InitFadingIn(StartFadingInEvent _)
        {
            if (VRSF_Components.VRCamera != null)
                _vrCamera = VRSF_Components.VRCamera.transform;
        }

        private void InitFadingOut(StartFadingOutEvent _)
        {
            if (VRSF_Components.VRCamera != null)
                _vrCamera = VRSF_Components.VRCamera.transform;
        }
    }
}