using Unity.Entities;

namespace VRSF.MoveAround.VRRotation
{
    public struct NonLinearUserRotation : IComponentData
    {
        /// <summary>
        /// Amount of degrees to rotate 
        /// </summary>
        public float DegreesToRotate;

        /// <summary>
        /// If the user already rotated this frame
        /// </summary>
        public bool HasAlreadyRotated;
    }
}