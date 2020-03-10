using Unity.Entities;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// TODO
    /// </summary>
    public struct StartClickingEventComp : IComponentData
    {
        /// <summary>
        /// TODO
        /// </summary>
        public EControllersButton ButtonInteracting;

        /// <summary>
        /// TODO
        /// </summary>
        public bool HasWaitedOneFrameBeforeRemoval;
    }
}