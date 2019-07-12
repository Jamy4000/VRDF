using Unity.Entities;

namespace VRSF.Core.Inputs
{
    public struct StopClickingEventComp : IComponentData
    {
        public EControllersButton ButtonInteracting;
    }
}