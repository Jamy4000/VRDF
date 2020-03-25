using Unity.Entities;

namespace VRDF.MoveAround.VRRotation
{
    public struct LinearRotationDeceleration : IComponentData
    {
        public float DecelerationFactor;
    }
}
