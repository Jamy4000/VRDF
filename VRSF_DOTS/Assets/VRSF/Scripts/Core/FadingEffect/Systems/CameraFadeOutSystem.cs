using Unity.Entities;

namespace VRSF.Core.FadingEffect
{
    public class CameraFadeOutSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            StartFadingOutEvent.Listeners += StartFadeOut;
            OnFadingOutEndedEvent.Listeners += OnFadeOutEnded;
            base.OnCreate();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StartFadingOutEvent.Listeners -= StartFadeOut;
            OnFadingOutEndedEvent.Listeners -= OnFadeOutEnded;
            this.Enabled = false;
        }

        private void StartFadeOut(StartFadingOutEvent info)
        {
            Entities.ForEach((ref CameraFadeParameters cameraFade) =>
            {
                cameraFade.FadingInProgress = true;
                cameraFade.IsFadingIn = false;
                cameraFade.ShouldImmediatlyFadeIn = info.ShouldFadeInWhenDone;
                cameraFade.OldFadingSpeedFactor = cameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    cameraFade.FadingSpeed = info.SpeedOverride;
            });
        }
        
        private void OnFadeOutEnded(OnFadingOutEndedEvent info)
        {
            Entities.ForEach((ref CameraFadeParameters cameraFade) =>
            {
                var overrideSpeed = cameraFade.FadingSpeed;

                CameraFadeSystem.ResetParameters(ref cameraFade);

                if (cameraFade.ShouldImmediatlyFadeIn)
                    new StartFadingInEvent(overrideSpeed);

                cameraFade.ShouldImmediatlyFadeIn = false;
            });
        }
    }
}