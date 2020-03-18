using Unity.Entities;
using VRDF.Core.Raycast;

namespace VRDF.Core.LaserPointer
{
    /// <summary>
    /// Make the Pointer appear only when it's hitting something
    /// </summary>
    public class LaserPointerVisibilitySystem : ComponentSystem
    {
        [Unity.Burst.BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach((ref LaserPointerState stateComp, ref LaserPointerVisibility visibilityComp, ref VRRaycastOrigin raycastOrigin) =>
            {
                switch (stateComp.State)
                {
                    case EPointerState.FORCE_OFF:
                        if (visibilityComp.CurrentWidth != 0.0f)
                        {
                            visibilityComp.CurrentWidth = 0.0f;
                            new OnLaserWidthChanged(raycastOrigin.RayOrigin, visibilityComp.CurrentWidth);
                        }
                        break;
                    case EPointerState.ON:
                        if (stateComp.StateJustChangedToOn)
                        {
                            stateComp.StateJustChangedToOn = false;
                            visibilityComp.CurrentWidth = visibilityComp.BaseWidth;
                            new OnLaserWidthChanged(raycastOrigin.RayOrigin, visibilityComp.BaseWidth);
                        }
                        break;

                    case EPointerState.DISAPPEARING:
                        visibilityComp.CurrentWidth -= (Time.DeltaTime * visibilityComp.DisappearanceSpeed) / 2000;

                        if (visibilityComp.CurrentWidth < 0.0f)
                            stateComp.State = EPointerState.OFF;

                        new OnLaserWidthChanged(raycastOrigin.RayOrigin, visibilityComp.CurrentWidth);
                        break;
                }
            });
        }
    }
}