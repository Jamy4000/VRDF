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