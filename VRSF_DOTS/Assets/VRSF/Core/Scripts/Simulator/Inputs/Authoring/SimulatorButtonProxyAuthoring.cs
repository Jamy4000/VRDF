using UnityEngine;
using Unity.Entities;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Simulate the click of a button on a controller, but using the Simulator
    /// </summary>
    [RequireComponent(typeof(VRInteractionAuthoring))]
    public class SimulatorButtonProxyAuthoring : MonoBehaviour
    {
        [Header("The button used to simulate a controller's button")]
        [Tooltip("Simply set this KeyCode and the VRInteractionAuthoring next to this component to be able to simulate the controller's button with the simulator")]
        [SerializeField] private KeyCode _simulationKeyCode;

        public void AddSimulatorButtonProxy(EntityManager entityManager, Entity createdEntity, VRInteractionAuthoring interactionParameters)
        {
            entityManager.AddComponentData(createdEntity, new SimulatorButtonProxy
            {
                SimulatedButton = interactionParameters.ButtonToUse,
                SimulationKeyCode = _simulationKeyCode
            });

            Destroy(this);
        }
    }
}