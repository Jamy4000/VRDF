using Unity.Entities;

namespace VRSF.Core.Inputs
{
    public struct StartTouchingEventComp : IComponentData
    {
        public EControllersButton ButtonInteracting;
    }
}