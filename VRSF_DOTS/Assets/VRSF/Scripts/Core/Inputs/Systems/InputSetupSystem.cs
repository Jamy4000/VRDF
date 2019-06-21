using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    public class InputSetupSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            if (VRSF_Components.SetupVRIsReady)
                CreateInputEntity();
        }
        
        /// <summary>
        /// Create the entity responsible to hold the Input Data.
        /// </summary>
        private void CreateInputEntity()
        {
            // Get the entityManager for this world
            var entityManager = World.Active.EntityManager;

            // We create an archetype with the correct input component data for the current loaded device
            EntityArchetype archetype = SetCorrectInputComponent();

            // We create the input entity based on the archetype created before
            Entity inputEntity = entityManager.CreateEntity(archetype);

            // We add the crossplatformInputCapture to the inputEntity and set the IsCheckingGrip bool based on the loaded device
            entityManager.AddComponentData(inputEntity, new CrossplatformInputCapture()
            {
                SqueezeClickThreshold = 0.95f
            });

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(inputEntity, string.Format("Input Capture Entity", inputEntity.Index));
#endif

            // Deactivate this system, as we only need it at runtime
            this.Enabled = false;



            /// <summary>
            /// Depending on the device loaded, we add a specific type of inputCaptureComponent in the archetype
            /// </summary>
            EntityArchetype SetCorrectInputComponent()
            {
                switch (VRSF_Components.DeviceLoaded)
                {
                    case EDevice.SIMULATOR:
                        return entityManager.CreateArchetype
                        (
                            typeof(SimulatorInputCaptureComponent)
                        );
                    case EDevice.OCULUS_RIFT:
                    case EDevice.OCULUS_RIFT_S:
                    case EDevice.OCULUS_QUEST:
                        return entityManager.CreateArchetype
                        (
                            typeof(OculusControllersInputCaptureComponent)
                        );
                    case EDevice.OCULUS_GO:
                    case EDevice.GEAR_VR:
                        return entityManager.CreateArchetype
                        (
                            typeof(GoAndGearVRControllersInputCaptureComponent)
                        );
                    case EDevice.HTC_VIVE:
                    case EDevice.HTC_FOCUS:
                        return entityManager.CreateArchetype
                        (
                            typeof(HtcControllersInputCaptureComponent)
                        );
                    case EDevice.WMR:
                        return entityManager.CreateArchetype
                        (
                            typeof(WMRControllersInputCaptureComponent)
                        );
                    default:
                        UnityEngine.Debug.LogError("[b]VRSF :[/b] No Device was loaded, an error must have occured in the DeviceLoaderSystem. " +
                            "Please contact me on Github or with my email adress, it's a bug from my side. Returning Simulator Archetype and setting device loaded to SImulator.");
                        VRSF_Components.DeviceLoaded = EDevice.SIMULATOR;

                        return entityManager.CreateArchetype
                        (
                            typeof(SimulatorInputCaptureComponent)
                        );
                }
            }
        }
    }
}