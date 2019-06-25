using E7.ECS.LineRenderer;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Make the Pointer appear only when it's hitting something
    /// </summary>
    public class LaserPointerVisibilitySystem : JobComponentSystem
    {
        #region ComponentSystem_Methods
        // Update is called once per frame
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new SetLineWidthJob
            {
                DeltaTime = Time.deltaTime
            }.Schedule(this, inputDeps);
        }
        #endregion ComponentSystem_Methods

        [Unity.Burst.BurstCompile]
        struct SetLineWidthJob : IJobForEach<LaserPointerState, LineSegment, LaserPointerVisibility, LaserPointerWidth>
        {
            public float DeltaTime;

            public void Execute(ref LaserPointerState stateComp, ref LineSegment lineSegment, ref LaserPointerVisibility visibilityComp, ref LaserPointerWidth widthComp)
            {
                switch (stateComp.State)
                {
                    case EPointerState.ON:
                        lineSegment.lineWidth = widthComp.BaseWidth;
                        break;

                    case EPointerState.DISAPPEARING:
                        lineSegment.lineWidth -= (DeltaTime * visibilityComp.DisappearanceSpeed) / 1000;

                        if (lineSegment.lineWidth < 0.0f)
                            stateComp.State = EPointerState.OFF;
                        break;
                }
            }
        }
    }
}