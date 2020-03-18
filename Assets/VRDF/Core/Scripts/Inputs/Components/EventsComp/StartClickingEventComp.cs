using Unity.Entities;

namespace VRDF.Core.Inputs
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