using Unity.Entities;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to capture the input from the X Button of an Oculus Rift, Rift S and Quest controller
    /// </summary>
    public struct XButtonInputCapture : IComponentData
    {
        /// <summary>
        /// Is the User clicking on the X Button
        /// </summary>
        public bool X_Click;

        /// <summary>
        /// Is the User touching the X Button
        /// </summary>
        public bool X_Touch;
    }
}