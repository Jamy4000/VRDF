using Unity.Entities;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Controllers
{
    /// <summary>
    /// Handle the switch of model between the two hands of the user when using Oculus Go or GearVR
    /// </summary>
    public class DominantHandHandlerSystem : ComponentSystem
    {
        private struct Filter
        {
            public GoAndGearVRControllersInputCaptureComponent SingleControllerInputCapture;
        }

        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            ChangeDominantHandEvent.Listeners += ChangeDominantHand;
            OnSetupVRReady.Listeners += Setup;
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            ChangeDominantHandEvent.Listeners -= ChangeDominantHand;
            OnSetupVRReady.Listeners -= Setup;
        }

        private void ChangeDominantHand(ChangeDominantHandEvent info)
        {
            if (VRSF_Components.DeviceLoaded != EDevice.GEAR_VR && VRSF_Components.DeviceLoaded != EDevice.OCULUS_GO)
            {
                UnityEngine.Debug.LogError("<b>[VRSF] :</b> This feature is only available for when you use the GearVR or Oculus Go.");
                return;
            }

            foreach (var e in GetEntities<Filter>())
            {
                e.SingleControllerInputCapture.IsUserRightHanded = info.NewDominantHand == EHand.RIGHT;
                DisableUnusedHand(e.SingleControllerInputCapture.IsUserRightHanded);
            }
        }

        private void Setup(OnSetupVRReady info)
        {
            if (VRSF_Components.DeviceLoaded != EDevice.GEAR_VR && VRSF_Components.DeviceLoaded != EDevice.OCULUS_GO)
                return;

            foreach (var e in GetEntities<Filter>())
            {
                DisableUnusedHand(e.SingleControllerInputCapture.IsUserRightHanded);
            }
        }

        private void DisableUnusedHand(bool isUserRightHanded)
        {
            VRSF_Components.LeftController.SetActive(!isUserRightHanded);
            VRSF_Components.RightController.SetActive(isUserRightHanded);
        }
    }
}