using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.VRInteractions
{
    /// <summary>
    /// Contains the variables for the PointerClickingSystem. 
    /// WARNING : This needs to be place on the same GameObject as where the VRRaycastAuthoring component is placed.
    /// </summary>
    [RequireComponent(typeof(Raycast.VRRaycastAuthoring))]
    public class PointerClickAuthoring : MonoBehaviour
    {
        public void AddPointerClickComponents(Entity entity)
        {
            var interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device list using this PointerClickAuthoring
            if ((interactionParameters.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                // We add a new pointer click to store
                entityManager.AddComponentData(entity, new PointerClick
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

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, string.Format("PointerClick Entity from GO {0}", transform.name));
#endif

                // If we use the simulator, we check for a SimulatorButtonProxy. if not null, we add the simulatorButtonProxy script
                if (VRSF_Components.DeviceLoaded == SetupVR.EDevice.SIMULATOR)
                    GetComponent<SimulatorButtonProxyAuthoring>()?.AddSimulatorButtonProxy(entityManager, entity, interactionParameters);

                Destroy(this);
            }
        }
    }

    public struct PointerClick : IComponentData
    {
        /// <summary>
        /// The button we want to check when clicking, default should be trigger
        /// </summary>
        public Inputs.EControllersButton ControllersButton;

        /// <summary>
        /// the hand used to check for click with this pointer
        /// </summary>
        public EHand HandClicking;

        /// <summary>
        /// Whether the user is able to click on stuffs
        /// </summary>
        public bool CanClick;
    }
}