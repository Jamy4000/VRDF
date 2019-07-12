using Unity.Entities;

namespace VRSF.Core.Inputs
{
    public struct StopTouchingEventComp : IComponentData
    {
        public EControllersButton ButtonInteracting;
    }
}