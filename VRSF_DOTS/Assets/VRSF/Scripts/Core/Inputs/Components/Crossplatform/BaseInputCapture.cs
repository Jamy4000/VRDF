using Unity.Entities;

namespace VRSF.Core.Inputs
{
    public struct BaseInputCapture : IComponentData
    {
        /// <summary>
        /// Is the User clicking on the button ?
        /// </summary>
        public bool IsClicking;

        /// <summary>
        /// Is the User touching the button ?
        /// </summary>
        public bool IsTouching;
    }
}
