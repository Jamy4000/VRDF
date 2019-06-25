using Unity.Entities;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the Y Button of an Oculus Rift, Rift S and Quest controller
    /// </summary>
    public struct YButtonInputCapture : IComponentData
    {
        /// <summary>
        /// Is the User clicking on the Y Button
        /// </summary>
        public bool Y_Click;

        /// <summary>
        /// Is the User touching the Y Button
        /// </summary>
        public bool Y_Touch;
    }
}