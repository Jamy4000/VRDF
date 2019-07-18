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
    /// Used when rotation doesn't use a deceleration, reset the variables
    /// WARNING Can give motion sickness !
    /// </summary>
    public class UserRotationStopperSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!VRSF_Components.SetupVRIsReady)
                return inputDeps;

            NativeArray<float3> rotationAxisOutput = new NativeArray<float3>(1, Allocator.TempJob);
            NativeArray<float> currentSpeedOutput = new NativeArray<float>(1, Allocator.TempJob);

            var job = new StopperJob().Schedule(this, inputDeps);

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

        [Unity.Burst.BurstCompile]
        [ExcludeComponent(typeof(LinearRotationDeceleration))]
        private struct StopperJob : IJobForEach<LinearUserRotation, UserRotationInteractionType, BaseInputCapture>
        {
            public void Execute(ref LinearUserRotation lur, ref UserRotationInteractionType urit, ref BaseInputCapture bic)
            {
                if (lur.CurrentRotationSpeed > 0.0f && ((urit.UseClickToRotate && !bic.IsClicking) || (urit.UseTouchToRotate && !bic.IsTouching)))
                {
                    // Setting the current speed of the user to 0
                    lur.CurrentRotationSpeed = 0.0f;
                }
            }
        }
    }
}