using Unity.Entities;
using Unity.Mathematics;

namespace VRSF.MoveAround.Teleport
{
    public struct CurveTeleporterCalculations : IComponentData
    {
        /// <summary>
        /// Initial velocity of the parabola, in local space.
        /// </summary>
        public float3 InitialVelocity;

        /// <summary>
        /// World-space \"acceleration\" of the parabola.  This effects the falloff of the curve.
        /// </summary>
        public float3 Acceleration;

        public int LastPointIndex;
        public bool PointOnNavMesh { get; set; }
        public bool PointOnTeleportableLayer { get; set; }
        public float CurrentParabolaAngleY { get; set; }
        public float3 CurrentPointVector { get; set; }

        /// <summary>
        /// Temporary point where we wanna go
        /// </summary>
        public float3 PointToGoTo;
    }
}
