using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// System calculating the direction in which the user is going
    /// </summary>
    
    public class FlyDirectionCalculationSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new FlyDirectionCalculationJob().Schedule(this, inputDeps);
        }
        
        [RequireComponentTag(typeof(IsFlying))]
        private struct FlyDirectionCalculationJob : IJobForEach<FlySpeed, FlyDirection, VRRaycastOutputs>
        {
            public void Execute(ref FlySpeed speed, ref FlyDirection direction, [ReadOnly] ref VRRaycastOutputs raycastOutputs)
            {
                var normalizeDir = Vector3.Normalize(raycastOutputs.RayVar.direction);
                direction.CurrentDirection = normalizeDir * speed.CurrentFlightVelocity * direction.FlightDirection;
            }
        }
    }
}