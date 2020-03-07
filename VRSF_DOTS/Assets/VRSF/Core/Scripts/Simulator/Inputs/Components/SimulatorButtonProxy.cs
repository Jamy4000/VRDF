using UnityEngine;
using Unity.Entities;

namespace VRSF.Core.Inputs
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

        /// <summary>
        /// The button used to simulate a controller's button
        /// Simply set this KeyCode and the VRInteractionAuthoring next to this component to be able to simulate the controller's button with the simulator" +
        /// </summary>
        public KeyCode SimulationKeyCode;
    }
}