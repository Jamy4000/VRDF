using Unity.Entities;

namespace VRSF.Core.Inputs
{
    public struct StartClickingEventComp : IComponentData
    {
        public EControllersButton ButtonInteracting;
    }
}