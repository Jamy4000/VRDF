using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeInSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += StartFadingIn;
            StartFadingInEvent.Listeners += StartFadeIn;
            OnFadingInEndedEvent.Listeners += OnFadeInEnded;
            base.OnCreate();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            OnSetupVRReady.Listeners -= StartFadingIn;
            StartFadingInEvent.Listeners -= StartFadeIn;
            OnFadingInEndedEvent.Listeners -= OnFadeInEnded;
            this.Enabled = false;
        }

        private void StartFadeIn(StartFadingInEvent info)
        {
            Entities.ForEach((ref CameraFadeParameters cameraFade) =>
            {
                cameraFade.FadingInProgress = true;
                cameraFade.IsFadingIn = true;
                cameraFade.OldFadingSpeedFactor = cameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    cameraFade.FadingSpeed = info.SpeedOverride;
            });
        }

        private void OnFadeInEnded(OnFadingInEndedEvent info)
        {
            Entities.ForEach((ref CameraFadeParameters cameraFade) =>
            {
                CameraFadeSystem.ResetParameters(ref cameraFade);
            });
        }
        
        private void StartFadingIn(OnSetupVRReady info)
        {
            new StartFadingInEvent(0.5f);
        }
    }
}