using Unity.Entities;

namespace VRDF.Core.Inputs
{
    public struct StartTouchingEventComp : IComponentData
    {
        public EControllersButton ButtonInteracting;

        public bool HasWaitedOneFrameBeforeRemoval;
    }
}