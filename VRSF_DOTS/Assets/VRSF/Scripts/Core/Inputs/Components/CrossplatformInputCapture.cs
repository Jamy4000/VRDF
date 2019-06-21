using Unity.Entities;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Required to have the references to the controllers parameters and capture the inputs for all platform
    /// </summary>
    public struct CrossplatformInputCapture : IComponentData
    {
        /// <summary>
        /// Until which point do we consider a squeeze on the grip and trigger as touch or as click
        /// </summary>
        public float SqueezeClickThreshold;
    }
}