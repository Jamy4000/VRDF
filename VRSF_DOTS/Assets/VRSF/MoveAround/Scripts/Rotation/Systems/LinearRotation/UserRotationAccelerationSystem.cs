using VRSF.Core.SetupVR;
using VRSF.Core.Inputs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.VRRotation
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
                DeltaTime = Time.DeltaTime,
                RotationAxis = rotationAxisOutput,
                CurrentSpeed = currentSpeedOutput
            }.Schedule(this, inputDeps);

            job.Complete();

            VRSF_Components.RotateVRCameraAround(rotationAxisOutput[0], currentSpeedOutput[0]);

            rotationAxisOutput.Dispose();
            currentSpeedOutput.Dispose();

            return inputDeps;
        }

        [Unity.Burst.BurstCompile]
        private struct RotationJob : IJobForEach<LinearUserRotation, ControllersInteractionType, BaseInputCapture, TouchpadInputCapture>
        {
            [ReadOnly] public float DeltaTime;

            public NativeArray<float3> RotationAxis;
            public NativeArray<float> CurrentSpeed;

            public void Execute(ref LinearUserRotation lur, [ReadOnly] ref ControllersInteractionType cit, [ReadOnly] ref BaseInputCapture bic, [ReadOnly] ref TouchpadInputCapture tic)
            {
                if (InteractionChecker.IsInteractingSimple(bic, cit))
                {
                    // maxSpeedTimeDeltaTime : To calculate the current speed according to deltaTime, Max Speed and acceleration factor
                    float maxSpeedTimeDeltaTime = DeltaTime * lur.AccelerationFactor * (lur.MaxRotationSpeed / 50);

                    // Setting the current speed of the user
                    lur.CurrentRotationSpeed += maxSpeedTimeDeltaTime;

                    lur.LastThumbXPos = tic.ThumbPosition.x;
                    RotationAxis[0] = new float3(0, lur.LastThumbXPos, 0);
                    CurrentSpeed[0] = lur.CurrentRotationSpeed;
                }
            }
        }
    }
}