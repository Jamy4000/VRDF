using UnityEngine;
using Unity.Entities;

namespace VRDF.Core.Simulator
{
    /// <summary>
    /// Simulate the click of a button on a controller, but using the Simulator and the Keyboard
    /// </summary>
    public struct SimulatorButtonKeyCode : IComponentData
    {
        /// <summary>
        /// The button used to simulate a controller's button
        /// Simply set this KeyCode and the VRInteractionAuthoring next to this component to be able to simulate the controller's button with the simulator" +
        /// </summary>
        public KeyCode SimulationKeyCode;
    }
}