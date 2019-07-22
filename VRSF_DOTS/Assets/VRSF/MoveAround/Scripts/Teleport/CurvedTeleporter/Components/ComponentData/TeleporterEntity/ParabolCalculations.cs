using Unity.Entities;
using Unity.Mathematics;

namespace VRSF.MoveAround.Teleport
{
    public struct ParabolCalculations : IComponentData
    {
        public float3 Velocity;
        public float3 Normal;
        public Core.Controllers.EHand Origin;
    }
}
