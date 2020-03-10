using Unity.Entities;
using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Simulator
{
    /// <summary>
    /// System to capture the keyboard keys provided by the SimulatorButtonProxy components
    /// </summary>
    public class SimulatorKeyboardButtonProxySystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref SimulatorButtonKeyCode keyCode, ref SimulatorButtonProxy proxy, ref VRInteractions.ControllersInteractionType interactionType, ref BaseInputCapture baseInput) =>
            {
                // If the user press the keyboard key used to simulate a controller's button
                if (Input.GetKeyDown(keyCode.SimulationKeyCode))
                {
                    // if the VRInteractionAuthoring has a Click Interaction set in editor
                    if (interactionType.HasClickInteraction)
                    {
                        // Add a StartClickingEventComponent to activate the other systems
                        EntityManager.AddComponentData(entity, new StartClickingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsClicking = true;
                    }

                    // if the VRInteractionAuthoring has a Touch Interaction set in editor
                    if (interactionType.HasTouchInteraction)
                    {
                        // Add a StartTouchingEventComp to activate the other systems
                        EntityManager.AddComponentData(entity, new StartTouchingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsTouching = true;
                    }
                }
                else if (Input.GetKeyUp(keyCode.SimulationKeyCode))
                {
                    // if the VRInteractionAuthoring has a Click Interaction set in editor
                    if (interactionType.HasClickInteraction)
                    {
                        // Add a StopClickingEventComp to activate the other systems
                        EntityManager.AddComponentData(entity, new StopClickingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsClicking = false;
                    }

                    // if the VRInteractionAuthoring has a Touch Interaction set in editor
                    if (interactionType.HasTouchInteraction)
                    {
                        // Add a StopTouchingEventComp to activate the other systems
                        EntityManager.AddComponentData(entity, new StopTouchingEventComp { HasWaitedOneFrameBeforeRemoval = false, ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsTouching = false;
                    }
                }

                // The StartTouchingEventComp and StartClickingEventComp are respectively removed in the PointerClick and CBRA System
            });
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
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}