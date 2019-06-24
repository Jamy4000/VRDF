using Unity.Entities;
using UnityEngine;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Make the Pointer appear only when it's hitting something
    /// </summary>
    public class LaserPointerVisibilitySystem : ComponentSystem
    {
        // Update is called once per frame
        protected override void OnUpdate()
        {
            Entities.ForEach(
                (ref LaserPointerState stateComp, 
                 ref LaserPointerVisibility visibilityComp, 
                 ref LaserPointerWidth widthComp) =>
            {
                switch (stateComp.State)
                {
                    case EPointerState.ON:
                        widthComp.CurrentWidth = widthComp.BaseWidth;
                        break;

                    case EPointerState.DISAPPEARING:
                        widthComp.CurrentWidth -= (Time.deltaTime * visibilityComp.DisappearanceSpeed) / 1000;

                        if (widthComp.CurrentWidth < 0.0f)
                            stateComp.State = EPointerState.OFF;
                        break;
                }
            });
        }
    }
}