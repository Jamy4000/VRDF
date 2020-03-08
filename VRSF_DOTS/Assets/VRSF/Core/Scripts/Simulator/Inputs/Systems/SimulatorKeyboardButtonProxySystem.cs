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
            Entities.ForEach((Entity entity, ref SimulatorButtonProxy proxy, ref VRInteractions.ControllersInteractionType interactionType, ref BaseInputCapture baseInput) =>
            {
                if (Input.GetKeyDown(proxy.SimulationKeyCode))
                {
                    if (interactionType.HasClickInteraction)
                    {
                        EntityManager.AddComponentData(entity, new StartClickingEventComp { ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsClicking = true;
                    }

                    if (interactionType.HasTouchInteraction)
                    {
                        EntityManager.AddComponentData(entity, new StartTouchingEventComp { ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsTouching = true;
                    }
                }
                else if (Input.GetKeyUp(proxy.SimulationKeyCode))
                {
                    if (interactionType.HasClickInteraction)
                    {
                        EntityManager.AddComponentData(entity, new StopClickingEventComp { ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsClicking = false;
                    }
                    if (interactionType.HasTouchInteraction)
                    {
                        EntityManager.AddComponentData(entity, new StopTouchingEventComp { ButtonInteracting = proxy.SimulatedButton });
                        baseInput.IsTouching = false;
                    }
                }
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