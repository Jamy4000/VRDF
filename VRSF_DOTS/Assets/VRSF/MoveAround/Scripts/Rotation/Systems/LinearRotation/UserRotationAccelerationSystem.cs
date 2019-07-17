using UnityEngine;
using VRSF.Core.SetupVR;
using VRSF.Core.Inputs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

namespace VRSF.MoveAround.Rotation
{
    /// <summary>
    /// Rotate the user based on the Speed parameter using a sliding effect.
    /// WARNING Can give motion sickness !
    /// </summary>
    public class UserRotationAccelerationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!VRSF_Components.SetupVRIsReady)
                return inputDeps;

            NativeArray<float3> rotationAxisOutput = new NativeArray<float3>(1, Allocator.TempJob);
            NativeArray<float> currentSpeedOutput = new NativeArray<float>(1, Allocator.TempJob);

            var job = new RotationJob
            {
                DeltaTime = Time.deltaTime,
                RotationAxis = rotationAxisOutput,
                CurrentSpeed = currentSpeedOutput
            }.Schedule(this, inputDeps);

            job.Complete();

            VRSF_Components.CameraRig.transform.RotateAround(VRSF_Components.VRCamera.transform.position, rotationAxisOutput[0], currentSpeedOutput[0]);

            rotationAxisOutput.Dispose();
            currentSpeedOutput.Dispose();

            return inputDeps;
        }

        //private void HandleRotationWithAcceleration(Filter entity)
        //{
        //    // If the user is not aiming to the UI
        //    if (!entity.RotationComp.RaycastHitVar.RaycastHitIsOnUI())
        //    {
        //        // isAccelerating : The user is Rotating (touching/clicking the thumbstick) and the currentSpeed is < (maxSpeed / 5)
        //        bool isAccelerating = entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed < (entity.RotationComp.MaxSpeed / 20);

        //        // isDecelerating : The user is not Rotating (not touching/clicking the thumbstick) and the currentSpeed is > 0
        //        bool isDecelerating = !entity.RotationComp.IsRotating && entity.RotationComp.CurrentSpeed > 0.0f;

        //        // maxSpeedTimeDeltaTime : To calculate the current speed according to deltaTime and Max Speed
        //        float maxSpeedTimeDeltaTime = Time.deltaTime * (entity.RotationComp.MaxSpeed / 50);

        //        // LastThumbPos : The last thumbPos of the user when rotating (touching/clicking the thumbstick) only 
        //        entity.RotationComp.LastThumbPos = entity.RotationComp.IsRotating ? entity.BACCalculations.ThumbPos.Value.x : entity.RotationComp.LastThumbPos;
                
        //        // Setting the current speed of the user
        //        entity.RotationComp.CurrentSpeed += isAccelerating ? maxSpeedTimeDeltaTime : -maxSpeedTimeDeltaTime;

        //        if (entity.RotationComp.CurrentSpeed > 0.0f)
        //        {
        //            Vector3 eyesPosition = VRSF_Components.VRCamera.transform.position;
        //            Vector3 rotationAxis = new Vector3(0, entity.RotationComp.LastThumbPos, 0);

        //            VRSF_Components.CameraRig.transform.RotateAround(eyesPosition, rotationAxis, entity.RotationComp.CurrentSpeed);
        //        }
        //    }
        //}

        private struct RotationJob : IJobForEach<LinearUserRotation, BaseInputCapture, TouchpadInputCapture>
        {
            [ReadOnly] public float DeltaTime;

            public NativeArray<float3> RotationAxis;
            public NativeArray<float> CurrentSpeed;

            public void Execute(ref LinearUserRotation lur, ref BaseInputCapture bic, ref TouchpadInputCapture tic)
            {
                if ((lur.UseClickToRotate && bic.IsClicking) || (lur.UseTouchToRotate && bic.IsTouching))
                {
                    // maxSpeedTimeDeltaTime : To calculate the current speed according to deltaTime and Max Speed
                    float maxSpeedTimeDeltaTime = DeltaTime * (lur.MaxRotationSpeed / 50);

                    // Setting the current speed of the user
                    lur.CurrentRotationSpeed += maxSpeedTimeDeltaTime;

                    RotationAxis[0] = new float3(0, tic.ThumbPosition.x, 0);
                    CurrentSpeed[0] = lur.CurrentRotationSpeed;
                }
            }
        }
    }
}