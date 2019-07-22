using Unity.Entities;

namespace VRSF.MoveAround.VRRotation
{
    public struct LinearRotationDeceleration : IComponentData
    {
        public float DecelerationFactor;
    }
}
