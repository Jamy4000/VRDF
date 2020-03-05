using Unity.Entities;

namespace VRSF.Core.FadingEffect
{
    /// <summary>
    /// Handle the callbacks linked to the Fade Out event (Start and End)
    /// </summary>
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
        }

        /// <summary>
        /// When a new FadeOut effect is called, we add a CameraFadeOut component to help the CameraFadeSystem know what it needs to do
        /// </summary>
        private void StartFadeOut(StartFadingOutEvent info)
        {
            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                cameraFade.ShouldImmediatlyFadeIn = info.ShouldFadeInWhenDone;
                cameraFade.OldFadingSpeedFactor = cameraFade.FadingSpeed;

                if (info.SpeedOverride != -1.0f)
                    cameraFade.FadingSpeed = info.SpeedOverride;

                EntityManager.AddComponentData(e, new CameraFadeOut());
            });
        }

        /// <summary>
        /// When the Fading Out event is done, we remove the CameraFadeOut event and reset the fading effect parameters.
        /// If a FadeIn effect is requested as soon as we're done with the FadeOut effect, then we raise the new event with the override speed specified before.
        /// </summary>
        private void OnFadeOutEnded(OnFadingOutEndedEvent info)
        {
            Entities.ForEach((Entity e, ref CameraFadeParameters cameraFade) =>
            {
                EntityManager.RemoveComponent<CameraFadeOut>(e);

                var overrideSpeed = cameraFade.FadingSpeed;

                CameraFadeSystem.ResetParameters(ref cameraFade);

                if (cameraFade.ShouldImmediatlyFadeIn)
                    new StartFadingInEvent(overrideSpeed);

                cameraFade.ShouldImmediatlyFadeIn = false;
            });
        }
    }
}