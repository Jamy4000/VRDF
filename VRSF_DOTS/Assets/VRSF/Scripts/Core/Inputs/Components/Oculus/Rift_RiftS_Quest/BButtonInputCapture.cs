using Unity.Entities;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the B Button of an Oculus Rift, Rift S and Quest controller
    /// </summary>
    public struct BButtonInputCapture : IComponentData
    {
        /// <summary>
        /// Is the User clicking on the B Button
        /// </summary>
        public bool B_Click;

        /// <summary>
        /// Is the User touching the B Button
        /// </summary>
        public bool B_Touch;
    }
}