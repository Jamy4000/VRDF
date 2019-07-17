using Unity.Entities;

namespace VRSF.MoveAround.Rotation
{
    public struct LinearUserRotation : IComponentData
    {
        public float CurrentRotationSpeed;
        public float MaxRotationSpeed;

        public bool UseClickToRotate;
        public bool UseTouchToRotate;
    }
}