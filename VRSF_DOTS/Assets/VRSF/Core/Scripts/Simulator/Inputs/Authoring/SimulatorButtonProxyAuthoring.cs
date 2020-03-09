using UnityEngine;
using Unity.Entities;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.Simulator
{
    /// <summary>
    /// Simulate the click of a button on a controller, but using the Simulator
    /// </summary>
    [RequireComponent(typeof(VRInteractionAuthoring))]
    public class SimulatorButtonProxyAuthoring : MonoBehaviour
    {
        [Header("The button used to simulate a controller's button")]
        [Tooltip("Should we use the left mouse button")]
        [SerializeField] public bool UseMouseButton;

        [Tooltip("Simply set this KeyCode and the VRInteractionAuthoring next to this component to be able to simulate the controller's button with the simulator")]
        [HideInInspector] public KeyCode SimulationKeyCode = KeyCode.None;

        [Tooltip("Simply set this KeyCode and the VRInteractionAuthoring next to this component to be able to simulate the controller's button with the simulator")]
        [HideInInspector] public EMouseButton SimulationMouseButton = EMouseButton.NONE;

        public void AddSimulatorButtonProxy(EntityManager entityManager, Entity createdEntity, VRInteractionAuthoring interactionParameters)
        {
            if (VRSF_Components.DeviceLoaded == SetupVR.EDevice.SIMULATOR)
            {
                if (UseMouseButton && SimulationMouseButton != EMouseButton.NONE)
                {
                    entityManager.AddComponentData(createdEntity, new SimulatorButtonMouse
                    {
                        SimulationMouseButton = SimulationMouseButton
                    });

                    AddSimulatorButtonProxyComp();
                }
                else if (!UseMouseButton && SimulationKeyCode != KeyCode.None)
                {
                    entityManager.AddComponentData(createdEntity, new SimulatorButtonKeyCode
                    {
                        SimulationKeyCode = SimulationKeyCode
                    });

                    AddSimulatorButtonProxyComp();
                }
            }

            Destroy(this);



            void AddSimulatorButtonProxyComp()
            {
                entityManager.AddComponentData(createdEntity, new SimulatorButtonProxy
                {
                    SimulatedButton = interactionParameters.ButtonToUse
                });
            }
        }
    }
}