using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    public class InputSetupSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += ReloadSystem;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            if (DeviceToLoad.IsLoaded)
            {
                var entityManager = World.Active.EntityManager;
                EntityArchetype archetype;

                switch (VRSF_Components.DeviceLoaded)
                {
                    case EDevice.SIMULATOR:
                        archetype = entityManager.CreateArchetype
                        (
                            typeof(CrossplatformInputCapture),
                            typeof(SimulatorInputCaptureComponent)
                        );
                        break;
                    case EDevice.OCULUS_RIFT:
                    case EDevice.OCULUS_RIFT_S:
                    case EDevice.OCULUS_QUEST:
                        archetype = entityManager.CreateArchetype
                        (
                            typeof(CrossplatformInputCapture),
                            typeof(OculusControllersInputCaptureComponent)
                        );
                        break;
                    case EDevice.OCULUS_GO:
                    case EDevice.GEAR_VR:
                        archetype = entityManager.CreateArchetype
                        (
                            typeof(CrossplatformInputCapture),
                            typeof(GoAndGearVRControllersInputCaptureComponent)
                        );
                        break;
                    case EDevice.HTC_VIVE:
                    case EDevice.HTC_FOCUS:
                        archetype = entityManager.CreateArchetype
                        (
                            typeof(CrossplatformInputCapture),
                            typeof(HtcControllersInputCaptureComponent)
                        );
                        break;
                    case EDevice.WMR:
                        archetype = entityManager.CreateArchetype
                        (
                            typeof(CrossplatformInputCapture),
                            typeof(WMRControllersInputCaptureComponent)
                        );
                        break;
                }

                var inputEntity = entityManager.CreateEntity(archetype);
#if UNITY_EDITOR
                entityManager.SetName(inputEntity, string.Format("Input Capture Entity", inputEntity.Index));
#endif
                this.Enabled = false;
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            OnSetupVRReady.Listeners -= ReloadSystem;
        }

        private void ReloadSystem(OnSetupVRReady info)
        {
            this.Enabled = true;
        }
    }
}