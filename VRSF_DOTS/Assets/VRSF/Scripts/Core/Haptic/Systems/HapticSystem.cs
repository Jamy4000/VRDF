using Unity.Entities;
using UnityEngine.XR;

namespace VRSF.Core.Controllers.Haptic
{
    /// <summary>
    /// When raising the OnHapticRequestedEvent, trigger an haptic pulse in the requested controller.
    /// </summary>
    public class HapticSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            OnHapticRequestedEvent.RegisterListener(OnHapticEventCallback);
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
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


            void SendImpulseToNode(XRNode node, float amplitude, float duration)
            {
                InputDevices.GetDeviceAtXRNode(node).SendHapticImpulse(0, amplitude, duration);
            }
        }
    }
}