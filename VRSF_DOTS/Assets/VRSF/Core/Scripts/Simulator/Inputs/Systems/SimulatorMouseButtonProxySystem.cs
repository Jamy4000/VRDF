using Unity.Entities;
using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Simulator
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
                int mouseButtonToCheck = -1;
                switch (mouseClick.SimulationMouseButton)
                {
                    case EMouseButton.LEFT_CLICK:
                        mouseButtonToCheck = 0;
                        break;
                    case EMouseButton.RIGHT_CLICK:
                        mouseButtonToCheck = 1;
                        break;
                    case EMouseButton.WHEEL_CLICK:
                        mouseButtonToCheck = 2;
                        break;
                }

                if (mouseButtonToCheck == -1)
                    throw new System.Exception();

                if (Input.GetMouseButtonDown(mouseButtonToCheck))
                {
                    AddStartClickingComp(interactionType.HasClickInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                    AddStartTouchingComp(interactionType.HasTouchInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                }
                else if (Input.GetMouseButtonUp(mouseButtonToCheck))
                {
                    AddStopClickingComp(interactionType.HasClickInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                    AddStopTouchingComp(interactionType.HasTouchInteraction, ref baseInput, ref entity, proxy.SimulatedButton);
                }
            });
        }

        private void AddStartClickingComp(bool hasClickInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            if (hasClickInteraction)
            {
                EntityManager.AddComponentData(entity, new StartClickingEventComp { ButtonInteracting = simulatedButton });
                baseInput.IsClicking = true;
            }
        }

        private void AddStartTouchingComp(bool hasTouchInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            if (hasTouchInteraction)
            {
                EntityManager.AddComponentData(entity, new StartTouchingEventComp { ButtonInteracting = simulatedButton });
                baseInput.IsTouching = true;
            }
        }

        private void AddStopClickingComp(bool hasClickInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            if (hasClickInteraction)
            {
                EntityManager.AddComponentData(entity, new StopClickingEventComp { ButtonInteracting = simulatedButton });
                baseInput.IsClicking = false;
            }
        }

        private void AddStopTouchingComp(bool hasTouchInteraction, ref BaseInputCapture baseInput, ref Entity entity, EControllersButton simulatedButton)
        {
            if (hasTouchInteraction)
            {
                EntityManager.AddComponentData(entity, new StopTouchingEventComp { ButtonInteracting = simulatedButton });
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
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}