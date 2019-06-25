using Unity.Entities;
using Unity.Jobs;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// System to handle the visibility of the Pointers based on whether it's hitting something
    /// </summary>
    public class LaserPointerStateSystem : JobComponentSystem
    {
        #region ComponentSystem_Methods
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new CheckPointerStateJob().Schedule(this, inputDeps);
        }
        #endregion ComponentSystem_Methods

        [Unity.Burst.BurstCompile]
        struct CheckPointerStateJob : IJobForEach<LaserPointerState, VRRaycastOutputs, LaserPointerVisibility>
        {
            public void Execute
                (ref LaserPointerState stateComp,
                 ref VRRaycastOutputs raycastOutputs, 
                 ref LaserPointerVisibility visibilityComp)
            {
                if (stateComp.State == EPointerState.ON && raycastOutputs.RaycastHitVar.IsNull)
                    stateComp.State = EPointerState.DISAPPEARING;
            }
        }
    }
}