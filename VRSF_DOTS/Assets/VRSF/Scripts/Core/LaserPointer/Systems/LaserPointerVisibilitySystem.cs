using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Make the Pointer appear only when it's hitting something
    /// </summary>
    public class LaserPointerVisibilitySystem : ComponentSystem
    {
        #region ComponentSystem_Methods
        [Unity.Burst.BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach((ref LaserPointerState stateComp, ref LaserPointerVisibility visibilityComp, ref LaserPointerWidth widthComp, ref VRRaycastOrigin raycastOrigin) =>
            {
                switch (stateComp.State)
                {
                    case EPointerState.ON:
                        if (stateComp.StateJustChangedToOn)
                        {
                            stateComp.StateJustChangedToOn = false;
                            new OnLaserWidthChanged(raycastOrigin.RayOrigin, widthComp.BaseWidth);
                        }
                        break;

                    case EPointerState.DISAPPEARING:
                        var newWidth = (Time.deltaTime * visibilityComp.DisappearanceSpeed) / 1000;

                        if (newWidth < 0.0f)
                            stateComp.State = EPointerState.OFF;

                        new OnLaserWidthChanged(raycastOrigin.RayOrigin, newWidth);
                        break;
                }
            });
        }
        #endregion ComponentSystem_Methods
    }
}