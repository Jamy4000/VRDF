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