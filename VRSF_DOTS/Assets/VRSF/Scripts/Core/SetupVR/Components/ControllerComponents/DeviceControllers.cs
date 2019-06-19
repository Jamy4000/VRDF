using Unity.Entities;

namespace VRSF.Core.SetupVR
{
    /// <summary>
    /// 
    /// </summary>
    public struct DeviceControllers : IComponentData
    {
        public EDevice Device;
        public Entity LeftControllerEntity;
        public Entity RightControllerEntity;
    }
}