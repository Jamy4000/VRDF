using Unity.Entities;

namespace VRDF.MoveAround.VRRotation
{
    public struct LinearUserRotation : IComponentData
    {
        public float CurrentRotationSpeed;
        public float MaxRotationSpeed;
        public float AccelerationFactor;
        public float LastThumbXPos;
    }
}