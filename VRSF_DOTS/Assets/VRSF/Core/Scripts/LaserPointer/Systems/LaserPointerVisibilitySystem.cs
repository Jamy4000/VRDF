using Unity.Entities;
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
                    case EPointerState.FORCE_OFF:
                        if (widthComp.CurrentWidth != 0.0f)
                        {
                            widthComp.CurrentWidth = 0.0f;
                            new OnLaserWidthChanged(raycastOrigin.RayOrigin, widthComp.CurrentWidth);
                        }
                        break;
                    case EPointerState.ON:
                        if (stateComp.StateJustChangedToOn)
                        {
                            stateComp.StateJustChangedToOn = false;
                            widthComp.CurrentWidth = widthComp.BaseWidth;
                            new OnLaserWidthChanged(raycastOrigin.RayOrigin, widthComp.BaseWidth);
                        }
                        break;

                    case EPointerState.DISAPPEARING:
                        widthComp.CurrentWidth -= (Time.DeltaTime * visibilityComp.DisappearanceSpeed) / 2000;

                        if (widthComp.CurrentWidth < 0.0f)
                            stateComp.State = EPointerState.OFF;

                        new OnLaserWidthChanged(raycastOrigin.RayOrigin, widthComp.CurrentWidth);
                        break;
                }
            });
        }
        #endregion ComponentSystem_Methods
    }
}