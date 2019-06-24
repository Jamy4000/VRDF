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

        
        struct CheckPointerStateJob : IJobForEach<LaserPointerState, VRRaycastOrigin, LaserPointerVisibility>
        {
            public void Execute
                (ref LaserPointerState stateComp,
                 ref VRRaycastOrigin raycastOrigin, 
                 ref LaserPointerVisibility visibilityComp)
            {
                if (stateComp.State == EPointerState.ON)
                {
                    switch (raycastOrigin.RayOrigin)
                    {
                        case ERayOrigin.LEFT_HAND:
                            if (LeftControllerRaycastData.RaycastHitVar.IsNull)
                                stateComp.State = EPointerState.DISAPPEARING;
                            break;
                        case ERayOrigin.RIGHT_HAND:
                            if (RightControllerRaycastData.RaycastHitVar.IsNull)
                                stateComp.State = EPointerState.DISAPPEARING;
                            break;
                        default:
                            UnityEngine.Debug.LogError("[b]VRSF :[\b] Please specify a correct parameter for all your Raycast Origin.");
                            break;
                    }
                }
            }
        }
    }
}