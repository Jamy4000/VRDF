using Unity.Entities;

namespace VRDF.Core.Inputs
{
    public struct StopTouchingEventComp : IComponentData
    {
        public EControllersButton ButtonInteracting;

        public bool HasWaitedOneFrameBeforeRemoval;
    }
}