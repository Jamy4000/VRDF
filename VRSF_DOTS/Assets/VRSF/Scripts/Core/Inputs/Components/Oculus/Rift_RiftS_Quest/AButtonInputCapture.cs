using Unity.Entities;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the A Button of an Oculus Rift, Rift S and Quest controller
    /// </summary>
    public struct AButtonInputCapture : IComponentData
    {
        /// <summary>
        /// Is the User clicking on the A Button
        /// </summary>
        public bool A_Click;

        /// <summary>
        /// Is the User touching the A Button
        /// </summary>
        public bool A_Touch;
    }
}