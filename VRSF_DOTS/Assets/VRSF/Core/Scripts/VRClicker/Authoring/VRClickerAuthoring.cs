using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.VRClicker
{
    /// <summary>
    /// Contains the variables for the VRClickingSystem. 
    /// WARNING : This needs to be place on the same GameObject as where the VRRaycastAuthoring component is placed.
    /// </summary>
    [RequireComponent(typeof(Raycast.VRRaycastAuthoring))]
    public class VRClickerAuthoring : MonoBehaviour
    {
        public void AddPointerClickComponents(ref Entity entity, ref EntityManager entityManager)
        {
            var interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device list using this PointerClickAuthoring
            if ((interactionParameters.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                // We add a new pointer click to store
                entityManager.AddComponentData(entity, new VRClicker
                {
                    ControllersButton = interactionParameters.ButtonToUse,
                    HandClicking = interactionParameters.ButtonHand,
                    CanClick = true
                });

                // Add the corresponding input, Hand and Interaction type component for the selected button. 
                // If the button wasn't chose correctly or any parameter was wrongly set, we destroy this entity and return.
                if (!InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, interactionParameters))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                // If we use the simulator, we check for a SimulatorButtonProxy. if not null, we add the simulatorButtonProxy script
                if (VRSF_Components.DeviceLoaded == SetupVR.EDevice.SIMULATOR)
                    GetComponent<Simulator.SimulatorButtonProxyAuthoring>()?.AddSimulatorButtonProxy(ref entityManager, ref entity, interactionParameters);

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, string.Format("VR Clicker Entity from GO {0}", transform.name));
#endif

                Destroy(this);
            }
        }
    }
}