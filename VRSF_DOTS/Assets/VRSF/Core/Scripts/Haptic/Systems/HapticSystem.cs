using Unity.Entities;
using UnityEngine.XR;

namespace VRSF.Core.Controllers.Haptic
{
    /// <summary>
    /// When raising the OnHapticRequestedEvent, trigger an haptic pulse in the requested controller.
    /// </summary>
    public class HapticSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            OnHapticRequestedEvent.RegisterListener(OnHapticEventCallback);
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnHapticRequestedEvent.UnregisterListener(OnHapticEventCallback);
        }

        /// <summary>
        /// Callback for when a Haptic Pulse is requested for SteamVR
        /// </summary>
        /// <param name="onHapticRequested"></param>
        private void OnHapticEventCallback(OnHapticRequestedEvent onHapticRequested)
        {
            XRNode hand = onHapticRequested.Hand == EHand.LEFT ? XRNode.LeftHand : XRNode.RightHand;

            if (InputDevices.GetDeviceAtXRNode(hand).isValid)
                SendImpulseToNode(hand, onHapticRequested.HapticAmplitude, onHapticRequested.HapticDuration);

            // If Oculus Go or GearVR, we send it to the two hands, just in case
            if (SetupVR.VRSF_Components.DeviceLoaded == SetupVR.EDevice.GEAR_VR || SetupVR.VRSF_Components.DeviceLoaded == SetupVR.EDevice.OCULUS_GO)
                SendImpulseToNode(hand == XRNode.LeftHand ? XRNode.RightHand : XRNode.LeftHand, onHapticRequested.HapticAmplitude, onHapticRequested.HapticDuration);


            void SendImpulseToNode(XRNode node, float amplitude, float duration)
            {
                InputDevices.GetDeviceAtXRNode(node).SendHapticImpulse(0, amplitude, duration);
            }
        }
    }
}