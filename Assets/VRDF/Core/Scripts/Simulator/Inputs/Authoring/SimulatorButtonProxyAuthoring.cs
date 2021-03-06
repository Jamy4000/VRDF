﻿using UnityEngine;
using Unity.Entities;
using VRDF.Core.VRInteractions;

namespace VRDF.Core.Simulator
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

        /// <summary>
        /// Check if the device loaded is the Simulator, and if everything is setup correctly in the editor,
        /// create a new Entity to check the Simulator Inputs.
        /// </summary>
        /// <param name="entityManager">Entity manager to use to add the componentData to the entity</param>
        /// <param name="createdEntity">The entity to which we want to add the SImulator Inputs Support</param>
        /// <param name="interactionParameters">The VRInteractionAuthoring script we want to simulate</param>
        public void AddSimulatorButtonProxy(ref EntityManager entityManager, ref Entity createdEntity, VRInteractionAuthoring interactionParameters)
        {
            // If the device loaded is the Simulator
            if (VRDF_Components.DeviceLoaded == SetupVR.EDevice.SIMULATOR)
            {
                if (UseMouseButton && SimulationMouseButton != EMouseButton.NONE)
                {
                    entityManager.AddComponentData(createdEntity, new SimulatorButtonMouse
                    {
                        SimulationMouseButton = SimulationMouseButton
                    });

                    AddSimulatorButtonProxyComp(ref entityManager, ref createdEntity);
                }
                else if (!UseMouseButton && SimulationKeyCode != KeyCode.None)
                {
                    entityManager.AddComponentData(createdEntity, new SimulatorButtonKeyCode
                    {
                        SimulationKeyCode = SimulationKeyCode
                    });

                    AddSimulatorButtonProxyComp(ref entityManager, ref createdEntity);
                }
            }

            Destroy(this);


            /// <summary>
            /// Add the basic SimulatorButtonProxy component, used in Keyboard and Mouse Input Systems
            /// </summary>
            void AddSimulatorButtonProxyComp(ref EntityManager dstManager, ref Entity entity)
            {
                dstManager.AddComponentData(entity, new SimulatorButtonProxy
                {
                    SimulatedButton = interactionParameters.ButtonToUse
                });
            }
        }
    } 
}