using Unity.Entities;
using Unity.Mathematics;

namespace VRSF.MoveAround.Teleport
{
    public struct StepByStepComponent : IComponentData
    {
        /// <summary>
        /// The distance in Meters to move the Camera for the step by step feature.
        /// </summary>
        public float DistanceStepByStep;

        /// <summary>
        /// The step height of the NavMesh for the Teleport feature.
        /// </summary>
        public float StepHeight;
    }
}
