using System;
using Unity.Entities;
using UnityEngine;
using VRDF.Core.Inputs;
using VRDF.Core.SetupVR;

namespace VRDF.Core.Simulator
{
    /// <summary>
    /// System to capture the Mouse Clicks provided by the SimulatorButtonProxy components
    /// </summary>
    public class SimulatorMouseButtonProxySystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref SimulatorButtonMouse mouseClick, ref SimulatorButtonProxy proxy, ref VRInteractions.ControllersInteractionType interactionType, ref BaseInputCapture baseInput) =>
            {
                int mouseButtonToCheck = GetMouseButtonIndex(mouseClick.SimulationMouseButton);

                // If the user press the mouse button used to simulate a controller's button
                if (Input.GetMouseButtonDown(mouseButtonToCheck))
                {
                    AddStartClickingComp(interactionType.HasClickInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                    AddStartTouchingComp(interactionType.HasTouchInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                }
                // If the user release the mouse button used to simulate a controller's button
                else if (Input.GetMouseButtonUp(mouseButtonToCheck))
                {
                    AddStopClickingComp(interactionType.HasClickInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                    AddStopTouchingComp(interactionType.HasTouchInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                }
            });
        }

        private int GetMouseButtonIndex(EMouseButton simulationMouseButton)
        {
            switch (simulationMouseButton)
            {
                case EMouseButton.LEFT_CLICK:
                    return 0;
                case EMouseButton.RIGHT_CLICK:
                    return 1;
                case EMouseButton.WHEEL_CLICK:
                    return 2;
                default:
                    throw new System.Exception();
            }
        }

        /// <summary>
        /// Add a StartClickingEventComp to activate the other systems
        /// </summary>
        /// <param name="hasClickInteraction"></param>
        /// <param name="baseInput"></param>
        /// <param name="entity"></param>
        /// <param name="simulatedButton"></param>
        private void AddStartClickingComp(bool hasClickInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            // if the VRInteractionAuthoring has a Click Interaction set in editor
            if (hasClickInteraction)
            {
                EntityManager.AddComponentData(entity, new StartClickingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = simulatedButton });
                baseInput.IsClicking = true;
            }
        }

        /// <summary>
        /// Add a StartTouchingEventComp to activate the other systems
        /// </summary>
        /// <param name="hasTouchInteraction"></param>
        /// <param name="baseInput"></param>
        /// <param name="entity"></param>
        /// <param name="simulatedButton"></param>
        private void AddStartTouchingComp(bool hasTouchInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            if (hasTouchInteraction)
            {
                EntityManager.AddComponentData(entity, new StartTouchingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = simulatedButton });
                baseInput.IsTouching = true;
            }
        }

        /// <summary>
        /// Add a StopClickingEventComp to activate the other systems
        /// </summary>
        /// <param name="hasClickInteraction"></param>
        /// <param name="baseInput"></param>
        /// <param name="entity"></param>
        /// <param name="simulatedButton"></param>
        private void AddStopClickingComp(bool hasClickInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            // if the VRInteractionAuthoring has a Click Interaction set in editor
            if (hasClickInteraction)
            {
                EntityManager.AddComponentData(entity, new StopClickingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = simulatedButton });
                baseInput.IsClicking = false;
            }
        }

        /// <summary>
        /// Add a StopTouchingEventComp to activate the other systems
        /// </summary>
        /// <param name="hasTouchInteraction"></param>
        /// <param name="baseInput"></param>
        /// <param name="entity"></param>
        /// <param name="simulatedButton"></param>
        private void AddStopTouchingComp(bool hasTouchInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            if (hasTouchInteraction)
            {
                EntityManager.AddComponentData(entity, new StopTouchingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = simulatedButton });
                baseInput.IsTouching = false;
            }
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        /// <summary>
        /// Check if we use the good device
        /// </summary>
        /// <param name="info"></param>
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRDF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}