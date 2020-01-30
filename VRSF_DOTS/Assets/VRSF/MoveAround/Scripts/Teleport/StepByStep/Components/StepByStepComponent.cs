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

        /// <summary>
        /// Meant for Debug, if true, a red ray will be displayed on your scene view with the directon of the SBS Calculations, and a blue one will be shown to display the Raycast done to check for the teleportable/NavMesh surface. DONT FORGET TO TURN GIZMOS ON !
        /// </summary>
        public bool DebugCalculationsRay;
    }
}
