using Unity.Mathematics;

namespace VRSF.MoveAround.Fly
{
    public struct FlyDirection : Unity.Entities.IComponentData
    {
        /// <summary>
        /// Between 1 and -1
        /// </summary>
        public float FlightDirection;

        public float3 NormalizedDir;
        public float3 FinalDirection;
    }
}