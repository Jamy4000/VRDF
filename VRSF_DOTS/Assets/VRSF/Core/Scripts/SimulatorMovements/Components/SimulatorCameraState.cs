using Unity.Entities;
using VRSF.Core.Utils;

namespace VRSF.Core.Simulator
{
    public struct SimulatorCameraState : IComponentData
    {
        public CameraState TargetCameraState;
        public CameraState InterpolatingCameraState;
    }
}
