using Unity.Entities;

namespace VRSF.MoveAround.Rotation
{
    public struct NonLinearUserRotation : IComponentData
    {
        /// <summary>
        /// Amount of degrees to rotate 
        /// </summary>
        public float DegreesToRotate;
    }
}