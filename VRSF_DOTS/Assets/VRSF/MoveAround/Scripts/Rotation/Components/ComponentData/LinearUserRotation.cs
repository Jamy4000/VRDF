using Unity.Entities;

namespace VRSF.MoveAround.Rotation
{
    public struct LinearUserRotation : IComponentData
    {
        public float CurrentRotationSpeed;
        public float MaxRotationSpeed;
        public float AccelerationFactor;
        public float LastThumbXPos;
    }
}