using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    public class WMRLeftControllerInputCaptureSystem : JobComponentSystem
    {
        private EndSimulationEntityCommandBufferSystem _endSimEcbSystem;

        private string _leftInput;

        protected override void OnCreate()
        {
            // Cache the EndSimulationEntityCommandBufferSystem in a field, so we don't have to get it every frame
            _endSimEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            OnSetupVRReady.Listeners += CheckForDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (!VRSF_Components.SetupVRIsReady)
                return inputDeps;

            var handle = new MenuInputCaptureJob()
            {
                MenuButtonDown = Input.GetButtonDown(_leftInput),
                MenuButtonUp = Input.GetButtonUp(_leftInput),
                Commands = _endSimEcbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, inputDeps);

            handle.Complete();
            return handle;
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckForDevice;
            base.OnDestroy();
        }

        [RequireComponentTag(typeof(LeftHand))]
        struct MenuInputCaptureJob : IJobForEachWithEntity<MenuInputCapture, BaseInputCapture>
        {
            [ReadOnly] public bool MenuButtonDown;
            [ReadOnly] public bool MenuButtonUp;

            public EntityCommandBuffer.Concurrent Commands;

            public void Execute(Entity entity, int index, ref MenuInputCapture menuInput, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (MenuButtonDown)
                {
                    Commands.AddComponent(index, entity, new StartClickingEventComp { ButtonInteracting = EControllersButton.MENU });
                    baseInput.IsClicking = true;
                }
                else if (MenuButtonUp)
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
        private void CheckForDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.WMR;
            _leftInput = UnityEngine.XR.XRSettings.loadedDeviceName == "OpenVR" ? "HtcLeftMenuClick" : "WMRLeftMenuClick";
        }
        #endregion PRIVATE_METHODS
    }
}