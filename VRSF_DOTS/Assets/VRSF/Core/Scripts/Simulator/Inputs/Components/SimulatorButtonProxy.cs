using Unity.Entities;
using VRSF.Core.Inputs;

namespace VRSF.Core.Simulator
{
    /// <summary>
    /// Simulate the click of a button on a controller, but using the Simulator
    /// </summary>
    public struct SimulatorButtonProxy : IComponentData
    {
        /// <summary>
        /// The button that's being simulated
        /// </summary>
        public EControllersButton SimulatedButton;
    }
}