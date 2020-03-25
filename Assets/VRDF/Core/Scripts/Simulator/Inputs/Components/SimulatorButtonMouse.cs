using Unity.Entities;

namespace VRDF.Core.Simulator
{
    /// <summary>
    /// Simulate the click of a button on a controller, but using the Simulator and a button on the mouse
    /// </summary>
    public struct SimulatorButtonMouse : IComponentData
    {
        /// <summary>
        /// The button used to simulate a controller's button on the mouse
        /// </summary>
        public EMouseButton SimulationMouseButton;
    }
}