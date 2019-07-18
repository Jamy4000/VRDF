using Unity.Entities;

namespace VRSF.MoveAround.Rotation
{
    public struct LinearRotationDeceleration : IComponentData
    {
        public float DecelerationFactor;
    }
}
