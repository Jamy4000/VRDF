using VRDF.Core.Inputs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using VRDF.Core.VRInteractions;

namespace VRDF.MoveAround.VRRotation
{
    /// <summary>
    /// Used when rotation doesn't use a deceleration, reset the variables
    /// WARNING Can give motion sickness !
    /// </summary>
    public class UserRotationStopperSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!VRDF_Components.SetupVRIsReady)
                return inputDeps;

            NativeArray<float3> rotationAxisOutput = new NativeArray<float3>(1, Allocator.TempJob);
            NativeArray<float> currentSpeedOutput = new NativeArray<float>(1, Allocator.TempJob);

            var job = new StopperJob().Schedule(this, inputDeps);

            job.Complete();

            VRDF_Components.CameraRig.transform.RotateAround(VRDF_Components.VRCamera.transform.position, rotationAxisOutput[0], currentSpeedOutput[0]);

            rotationAxisOutput.Dispose();
            currentSpeedOutput.Dispose();

            return inputDeps;
        }

        [Unity.Burst.BurstCompile]
        [ExcludeComponent(typeof(LinearRotationDeceleration))]
        private struct StopperJob : IJobForEach<LinearUserRotation, ControllersInteractionType, BaseInputCapture>
        {
            public void Execute(ref LinearUserRotation lur, [ReadOnly] ref ControllersInteractionType cit, [ReadOnly] ref BaseInputCapture bic)
            {
                if (lur.CurrentRotationSpeed > 0.0f && InteractionChecker.IsNotInteracting(bic, cit))
                {
                    // Setting the current speed of the user to 0
                    lur.CurrentRotationSpeed = 0.0f;
                }
            }
        }
    }
}