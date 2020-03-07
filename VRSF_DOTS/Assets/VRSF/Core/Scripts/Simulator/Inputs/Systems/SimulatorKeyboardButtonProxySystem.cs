using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
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
            Entities.ForEach((Entity entity, ref SimulatorButtonProxy proxy, ref VRInteractions.ControllersInteractionType interactionType) =>
            {
                if (Input.GetKeyDown(proxy.SimulationKeyCode))
                {
                    if (interactionType.HasClickInteraction)
                        EntityManager.AddComponentData(entity, new StartClickingEventComp { ButtonInteracting = proxy.SimulatedButton });
                    if (interactionType.HasTouchInteraction)
                        EntityManager.AddComponentData(entity, new StartTouchingEventComp { ButtonInteracting = proxy.SimulatedButton });
                }
                else if (Input.GetKey(proxy.SimulationKeyCode))
                {
                    if (interactionType.HasClickInteraction)
                        EntityManager.AddComponentData(entity, new IsClickingComp { ButtonInteracting = proxy.SimulatedButton } );
                    if (interactionType.HasTouchInteraction)
                        EntityManager.AddComponentData(entity, new StartTouchingEventComp());
                }
                else if (Input.GetKeyUp(proxy.SimulationKeyCode))
                {
                    if (interactionType.HasClickInteraction)
                        EntityManager.AddComponentData(entity, new StartClickingEventComp());
                    if (interactionType.HasTouchInteraction)
                        EntityManager.AddComponentData(entity, new StartTouchingEventComp());
                }
            });
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct SimulatorInputCaptureJob : IJobForEachWithEntity<MenuInputCapture, BaseInputCapture>
        {
            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref SimulatorButtonProxy menuInput, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (EscapeButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.MENU });
                    baseInput.IsClicking = true;
                }
                else if (EscapeButtonUp)
                {
                    Commands.AddComponent(index, entity, new StopClickingEventComp { ButtonInteracting = EControllersButton.MENU });
                    baseInput.IsClicking = false;
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if we use the good device
        /// </summary>
        /// <param name="info"></param>
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}